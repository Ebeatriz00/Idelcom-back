using Core.Attributes;
using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Infrastructure.Persistence;
using SharedKernel.Constants;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;

namespace Application.Services.Audit
{
    public class AuditService(IAuditRepository auditRepository, ISqlConnectionFactory sqlConnectionFactory) : IAuditService
    {
        private readonly IAuditRepository _auditRepository = auditRepository;
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        // Cache de propiedades públicas para no repetir reflexión por tipo.
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

        // Registra el snapshot inicial de una entidad usando solo propiedades marcadas con [AuditField].
        public async Task RegisterCreateAsync<T>(
            T entity,
            AuditLog auditLog,
            IDbTransaction? trans = null,
            params string[] additionalIgnoredFields)
        {
            if (entity == null) return;

            auditLog.ActionType = AuditActionTypes.Insert;

            var properties = GetAuditableProperties(typeof(T), additionalIgnoredFields);
            var details = new List<AuditLogDetail>();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);

                if (ShouldSkipCreateValue(value))
                    continue;

                details.Add(CreateDetail(prop, null, value, auditLog.CreateUser));
            }

            if (details.Count == 0) return;

            await SaveAuditAsync(auditLog, details, trans);
        }

        // Registra únicamente los cambios detectados entre el estado anterior y el nuevo.
        public async Task RegisterUpdateAsync<T>(
            T before,
            T after,
            AuditLog auditLog,
            IDbTransaction? trans = null,
            params string[] additionalIgnoredFields)
        {
            if (before == null || after == null) return;

            auditLog.ActionType = AuditActionTypes.Update;

            var details = Compare(before, after, auditLog.CreateUser, additionalIgnoredFields);

            if (details.Count == 0) return;

            await SaveAuditAsync(auditLog, details, trans);
        }

        // Registra una eliminación lógica como cambio de Status hacia "0".
        public async Task RegisterDeleteAsync<T>(
            T before,
            AuditLog auditLog,
            IDbTransaction? trans = null)
        {
            if (before == null) return;

            auditLog.ActionType = AuditActionTypes.Delete;

            var statusProp = GetPublicProperties(typeof(T))
                .FirstOrDefault(p => p.Name.Equals("Status", StringComparison.OrdinalIgnoreCase));

            if (statusProp == null || !statusProp.CanRead) return;

            var oldValue = statusProp.GetValue(before);
            var oldValueStr = oldValue?.ToString();

            if (string.IsNullOrWhiteSpace(oldValueStr) || oldValueStr == "0") return;

            var detail = CreateDetail(statusProp, oldValue, "0", auditLog.CreateUser);

            if (detail.FieldAlias == "Status")
                detail.FieldAlias = "Estado";

            await SaveAuditAsync(auditLog, new List<AuditLogDetail> { detail }, trans);
        }

        // Compara dos instancias del mismo tipo y devuelve solo los campos auditables que cambiaron.
        private List<AuditLogDetail> Compare<T>(T before, T after, long createUser, params string[] additionalIgnoredFields)
        {
            var result = new List<AuditLogDetail>();
            var properties = GetAuditableProperties(typeof(T), additionalIgnoredFields);

            foreach (var prop in properties)
            {
                var oldValue = prop.GetValue(before);
                var newValue = prop.GetValue(after);

                if (!AreDifferent(oldValue, newValue))
                    continue;

                result.Add(CreateDetail(prop, oldValue, newValue, createUser));
            }

            return result;
        }

        // Devuelve solo propiedades marcadas con [AuditField], excluyendo las ignoradas por el caller.
        private IEnumerable<PropertyInfo> GetAuditableProperties(Type type, string[]? additionalIgnored)
        {
            var ignored = BuildIgnoredSet(additionalIgnored);
            var props = GetPublicProperties(type);

            return props.Where(p =>
                p.CanRead &&
                p.GetCustomAttribute<AuditFieldAttribute>() != null &&
                !ignored.Contains(p.Name));
        }

        // Crea el detalle persistible que representa un cambio de auditoría.
        private AuditLogDetail CreateDetail(PropertyInfo prop, object? oldVal, object? newVal, long userId)
        {
            return new AuditLogDetail
            {
                FieldName = prop.Name,
                FieldAlias = ResolveFieldAlias(prop),
                OldValue = FormatValue(oldVal),
                NewValue = FormatValue(newVal),
                CreateUser = userId,
                Status = "1"
            };
        }

        // Persiste cabecera y detalle de auditoría en una sola transacción.
        private async Task SaveAuditAsync(AuditLog auditLog, List<AuditLogDetail> details, IDbTransaction? trans)
        {
            if (trans == null)
            {
                using var connection = _sqlConnectionFactory.CreateConnection();
                await connection.OpenAsync();
                using var ownTransaction = await connection.BeginTransactionAsync();

                try
                {
                    await PersistAuditAsync(auditLog, details, ownTransaction);

                    await ownTransaction.CommitAsync();
                    return;
                }
                catch
                {
                    await ownTransaction.RollbackAsync();
                    throw;
                }
            }

            await PersistAuditAsync(auditLog, details, trans!);
        }

        // Inserta cabecera y detalles reutilizando la misma transacción recibida.
        private async Task PersistAuditAsync(AuditLog auditLog, List<AuditLogDetail> details, IDbTransaction trans)
        {
            var auditLogId = await _auditRepository.InsertAuditLogAsync(auditLog, trans);
            await _auditRepository.InsertAuditLogDetailAsync(
                auditLogId,
                auditLog.CreateUser,
                auditLog.BusinessId,
                details,
                trans);
        }

        // Obtiene propiedades públicas desde cache para evitar reflexión repetida.
        private static PropertyInfo[] GetPublicProperties(Type type)
        {
            return _propertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
        }

        // Construye el set de exclusión explícito recibido por el caller.
        private static HashSet<string> BuildIgnoredSet(string[]? additionalIgnored)
        {
            return additionalIgnored == null || additionalIgnored.Length == 0
                ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(additionalIgnored, StringComparer.OrdinalIgnoreCase);
        }

        // Resuelve el alias amigable de un campo auditado.
        private string ResolveFieldAlias(PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute<AuditFieldAttribute>();

            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Alias))
                return attribute.Alias;

            return prop.Name;
        }

        // Determina si dos valores representan un cambio relevante para auditoría.
        private bool AreDifferent(object? oldValue, object? newValue)
        {
            if (ReferenceEquals(oldValue, newValue)) return false;

            if (oldValue is string || newValue is string)
            {
                var normalizedOld = NormalizeString(oldValue as string);
                var normalizedNew = NormalizeString(newValue as string);

                return !string.Equals(normalizedOld, normalizedNew, StringComparison.Ordinal);
            }

            if (oldValue == null || newValue == null) return true;

            if (oldValue is decimal oldDecimal && newValue is decimal newDecimal)
                return Math.Round(oldDecimal, 2) != Math.Round(newDecimal, 2);

            if (oldValue is DateTime oldDate && newValue is DateTime newDate)
                return oldDate.ToString("yyyyMMddHHmmss") != newDate.ToString("yyyyMMddHHmmss");

            return !Equals(oldValue, newValue);
        }

        // Formatea valores primitivos para persistirlos de forma homogénea en auditoría.
        private string? FormatValue(object? value)
        {
            if (value == null) return null;

            return value switch
            {
                DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
                TimeSpan ts => ts.ToString(@"hh\:mm"),
                bool b => b ? "Si" : "No",
                decimal d => d.ToString("0.##"),
                _ => value.ToString()
            };
        }

        // Normaliza strings para evitar ruido por espacios o vacíos equivalentes.
        private string? NormalizeString(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        // Evita registrar valores vacíos en eventos de creación.
        private bool ShouldSkipCreateValue(object? value)
        {
            if (value == null) return true;
            if (value is string s && string.IsNullOrWhiteSpace(s)) return true;
            return false;
        }

    }
}
