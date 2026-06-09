using Core.Interfaces.Ssoma;
using Core.Interfaces.Ssoma.Clinics;
using Infrastructure.Exceptions;

namespace Application.UseCases.SsomaHomologationPersonnelDocument
{
    public class SsomaHomologationPersonnelDocumentBusinessRules(
        ISsomaHomologationPersonnelRepository homologationPersonnelRepository,
        ISsomaRequirementRepository requirementRepository,
        Core.Interfaces.IWorkerRepository workerRepository,
        IClinicRepository clinicRepository)
    {
        private readonly ISsomaHomologationPersonnelRepository _homologationPersonnelRepository = homologationPersonnelRepository;
        private readonly ISsomaRequirementRepository _requirementRepository = requirementRepository;
        private readonly Core.Interfaces.IWorkerRepository _workerRepository = workerRepository;
        private readonly IClinicRepository _clinicRepository = clinicRepository;

        public void Normalize(
            string? fileName,
            string? fileUrl,
            string? filePath,
            DateTime? issueDate,
            DateTime? expirationDate,
            DateTime? reviewDate,
            string? observation,
            out string? normalizedFileName,
            out string? normalizedFileUrl,
            out string? normalizedFilePath,
            out DateTime? normalizedIssueDate,
            out DateTime? normalizedExpirationDate,
            out DateTime? normalizedReviewDate,
            out string normalizedObservation)
        {
            normalizedFileName = string.IsNullOrWhiteSpace(fileName) ? null : fileName.Trim();
            normalizedFileUrl = string.IsNullOrWhiteSpace(fileUrl) ? null : fileUrl.Trim();
            normalizedFilePath = string.IsNullOrWhiteSpace(filePath) ? null : filePath.Trim();
            normalizedIssueDate = issueDate?.Date;
            normalizedExpirationDate = expirationDate?.Date;
            normalizedReviewDate = reviewDate?.Date;
            normalizedObservation = observation?.Trim() ?? string.Empty;
        }

        public async Task ValidateReferencesAsync(
            long businessId,
            long homologationPersonnelId,
            int requirementId,
            string fileName,
            DateTime? issueDate,
            DateTime? expirationDate,
            DateTime? reviewDate)
        {
            var homologationPersonnel = await _homologationPersonnelRepository.GetByIdAsync(homologationPersonnelId, businessId);
            if (homologationPersonnel == null)
                throw new BusinessException("La homologación de personal SSOMA no existe o no pertenece a la empresa actual.");

            var requirement = await _requirementRepository.GetByIdAsync(requirementId, businessId);
            if (requirement == null)
                throw new BusinessException("El requerimiento SSOMA no existe o no pertenece a la empresa actual.");

            if (!requirement.IsActive)
                throw new BusinessException("El requerimiento SSOMA se encuentra inactivo.");

            if (requirement.ScopeId != homologationPersonnel.HomologationScopeId)
                throw new BusinessException("El requerimiento SSOMA no es compatible con el alcance de la homologación de personal.");

            if (requirement.RequiresFile && string.IsNullOrWhiteSpace(fileName))
                throw new BusinessException("El requerimiento SSOMA exige un archivo adjunto válido.");

            if (requirement.RequiresExpiration)
            {
                if (!expirationDate.HasValue)
                    throw new BusinessException("La fecha de expiración es obligatoria para este requerimiento.");

                if (!issueDate.HasValue)
                    throw new BusinessException("La fecha de emisión es obligatoria para este requerimiento.");

                if (expirationDate.Value < issueDate.Value)
                    throw new BusinessException("La fecha de expiración no puede ser menor que la fecha de emisión.");
            }

            if (reviewDate.HasValue && issueDate.HasValue && reviewDate.Value < issueDate.Value)
                throw new BusinessException("La fecha de revisión no puede ser menor que la fecha de emisión.");

            ValidateFileExtension(requirement.AllowedExtensions, fileName);
        }

        public async Task ValidateListFilterAsync(long businessId, long? homologationPersonnelId, int? requirementId)
        {
            if (homologationPersonnelId.HasValue)
            {
                if (homologationPersonnelId.Value <= 0)
                    throw new BusinessException("La homologación de personal indicada no es válida.");

                var homologationPersonnel = await _homologationPersonnelRepository.GetByIdAsync(homologationPersonnelId.Value, businessId);
                if (homologationPersonnel == null)
                    throw new BusinessException("La homologación de personal SSOMA no existe o no pertenece a la empresa actual.");
            }

            if (requirementId.HasValue)
            {
                if (requirementId.Value <= 0)
                    throw new BusinessException("El requerimiento SSOMA indicado no es válido.");

                var requirement = await _requirementRepository.GetByIdAsync(requirementId.Value, businessId);
                if (requirement == null)
                    throw new BusinessException("El requerimiento SSOMA no existe o no pertenece a la empresa actual.");
            }
        }

        public async Task<DateTime?> ResolveExpirationDateAsync(long businessId, int requirementId, DateTime? issueDate, DateTime? expirationDate)
        {
            var requirement = await _requirementRepository.GetByIdAsync(requirementId, businessId)
                ?? throw new BusinessException("El requerimiento SSOMA no existe o no pertenece a la empresa actual.");

            if (requirement.RequiresExpiration)
            {
                return expirationDate?.Date;
            }

            return null;
        }

        public async Task<string> GetUploadPathAsync(long businessId, long homologationPersonnelId, int requirementId, long? clinicId = null)
        {
            var homologation = await _homologationPersonnelRepository.GetByIdAsync(homologationPersonnelId, businessId)
                ?? throw new BusinessException("La homologación de personal SSOMA no existe.");

            var requirement = await _requirementRepository.GetByIdAsync(requirementId, businessId)
                ?? throw new BusinessException("El requerimiento SSOMA no existe.");

            var worker = await _workerRepository.GetByIdAsync(homologation.WorkerId)
                ?? throw new BusinessException("El trabajador no existe.");

            string scopeText = requirement.ScopeId == 1 ? "INTERNO" : "PROYECTO";
            string workerFullName = $"{worker.WorkerName} {worker.WorkerLastName}".Trim();
            
            string basePath = $"OPERACIONES/SSOMA/HOMOLOGACIONES/{workerFullName}/{scopeText}";

            if (clinicId.HasValue)
            {
                var clinic = await _clinicRepository.GetByIdAsync(clinicId.Value, businessId);
                if (clinic != null)
                {
                    string clinicName = clinic.ClinicName?.Trim().ToUpperInvariant() ?? "SIN_NOMBRE";
                    basePath = $"{basePath}/EMO/{clinicName}";
                }
            }

            return basePath;
        }

        private static void ValidateFileExtension(string? allowedExtensions, string fileName)
        {
            if (string.IsNullOrWhiteSpace(allowedExtensions) || string.IsNullOrWhiteSpace(fileName))
                return;

            var extension = Path.GetExtension(fileName)?.Trim().TrimStart('.').ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension))
                return;

            var allowed = allowedExtensions
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.Trim().TrimStart('.').ToLowerInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet();

            if (allowed.Count == 0)
                return;

            if (!allowed.Contains(extension))
                throw new BusinessException("La extensión del archivo no está permitida para el requerimiento SSOMA.");
        }
    }
}
