using Core.Entities.Audit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Audit
{
    public interface IAuditService
    {
        /** <summary>
         * Registra una operación de actualización en el sistema de auditoría.
         * Permite especificar campos adicionales a ignorar.
         * </summary>
         * <typeparam name="T">El tipo del objeto que se está actualizando.</typeparam>
         * <param name="before">El objeto antes de la actualización.</param>
         * <param name="after">El objeto después de la actualización.</param>
         * <param name="auditLog">La información general del registro de auditoría (como BusinessId, TableName, RecordId, CreateUser).</param>
         * <param name="trans">Una transacción opcional para usar en la operación de auditoría.</param>
         * <param name="additionalIgnoredFields">Campos adicionales a ignorar en la comparación, además de los campos predeterminados.</param>
         * <returns>Una tarea que representa la operación asincrónica.</returns>
         */
        Task RegisterUpdateAsync<T>(
            T before,
            T after,
            AuditLog auditLog,
            IDbTransaction? trans = null,
            params string[] additionalIgnoredFields);

        /** <summary>
         * Registra una operación de creación en el sistema de auditoría.
         * Permite especificar campos adicionales a ignorar.
         * </summary>
         * <typeparam name="T">El tipo del objeto que se está creando.</typeparam>
         * <param name="entity">El objeto que se está creando.</param>
         * <param name="auditLog">La información general del registro de auditoría (como BusinessId, TableName, RecordId, CreateUser).</param>
         * <param name="trans">Una transacción opcional para usar en la operación de auditoría.</param>
         * <param name="additionalIgnoredFields">Campos adicionales a ignorar en la comparación, además de los campos predeterminados.</param>
         * <returns>Una tarea que representa la operación asincrónica.</returns>
         */
        Task RegisterCreateAsync<T>(
            T entity,
            AuditLog auditLog,
            IDbTransaction? trans = null,
            params string[] additionalIgnoredFields);

        /** <summary>
         * Registra una operación de eliminación en el sistema de auditoría.
         * </summary>
         * <typeparam name="T">El tipo del objeto que se está eliminando.</typeparam>
         * <param name="before">El objeto antes de la eliminación.</param>
         * <param name="auditLog">La información general del registro de auditoría (como BusinessId, TableName, RecordId, CreateUser).</param>
         */
        Task RegisterDeleteAsync<T>(
            T before,
            AuditLog auditLog,
            IDbTransaction? trans = null);
    }
}
