using Application.DTOs.SsomaHomologationPersonnel;
using Application.UseCases.SsomaHomologationPersonnel.services;
using Core.Interfaces.Operations;
using Core.Interfaces.Ssoma;
using FluentValidation;
using SharedKernel;
using AppValidationException = Application.Exceptions.ValidationException;
using DocumentCreateDto = Application.DTOs.SsomaHomologationPersonnelDocument.SsomaHomologationPersonnelDocumentCreateDto;

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class CreateSsomaHomologationPersonnelOrchestrated(
        CreateSsomaHomologationPersonnel createSsomaHomologationPersonnel,
        Application.UseCases.SsomaHomologationPersonnelDocument.CreateSsomaHomologationPersonnelDocument createSsomaHomologationPersonnelDocument,
        IValidator<SsomaHomologationPersonnelCreateOrchestratedDto> validator,
        ISsomaHomologationCalculator homologationCalculator,
        IOperationsRepository operationsRepository,
        ISsomaRequirementRepository requirementRepository)
    {
        private readonly CreateSsomaHomologationPersonnel _createSsomaHomologationPersonnel = createSsomaHomologationPersonnel;
        private readonly Application.UseCases.SsomaHomologationPersonnelDocument.CreateSsomaHomologationPersonnelDocument _createSsomaHomologationPersonnelDocument = createSsomaHomologationPersonnelDocument;
        private readonly IValidator<SsomaHomologationPersonnelCreateOrchestratedDto> _validator = validator;
        private readonly ISsomaHomologationCalculator _homologationCalculator = homologationCalculator;
        private readonly IOperationsRepository _operationsRepository = operationsRepository;
        private readonly ISsomaRequirementRepository _requirementRepository = requirementRepository;
        private const long ScopeOperation = 2;
        private const int CamoRequirementId = 12;
        private const int ValidationStatusValid = 2;


        public async Task<BaseResponseId> ExecuteAsync(SsomaHomologationPersonnelCreateOrchestratedDto dto, long userId, long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            DateTime? operationEndDate = null;

            if (dto.HomologationPersonnel.HomologationScopeId == ScopeOperation)
            {
                if (!dto.HomologationPersonnel.OperationsId.HasValue)
                    throw new AppValidationException(new List<GlobalErrorDetail>
                    {
                        new("OPERATIONS_REQUIRED", "La operación es obligatoria para homologación por operación.")
                    });

                operationEndDate = await _operationsRepository.GetOperationEndDateAsync(
                    dto.HomologationPersonnel.OperationsId.Value);

                if (!operationEndDate.HasValue)
                {
                    throw new AppValidationException(new List<GlobalErrorDetail>
                    {
                        new("OPERATION_END_DATE_NOT_FOUND", "No se encontró la fecha fin de la operación.")
                    });
                }
            }

            var documents = (dto.Documents ?? new List<DocumentCreateDto>())
                .Select(document => new DocumentCreateDto
                {
                    RequirementId = document.RequirementId,
                    FileName = document.FileName,
                    FileUrl = document.FileUrl,
                    FilePath = document.FilePath,
                    IssueDate = document.IssueDate,
                    ExpirationDate = document.ExpirationDate,
                    ValidationStatusId = ValidationStatusValid,
                    ReviewDate = document.ReviewDate,
                    Observation = document.Observation
                })
                .ToList();

            var documentErrors = new List<GlobalErrorDetail>();
            var duplicatedRequirementIds = documents
                .GroupBy(document => document.RequirementId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            foreach (var requirementId in duplicatedRequirementIds)
            {
                documentErrors.Add(new GlobalErrorDetail(
                    "DUPLICATED_REQUIREMENT",
                    $"No se puede registrar mas de un documento activo para el requerimiento {requirementId}."));
            }

            foreach (var document in documents)
            {
                var requirement = await _requirementRepository.GetByIdAsync(document.RequirementId, businessId);
                if (requirement == null)
                {
                    documentErrors.Add(new GlobalErrorDetail(
                        "REQUIREMENT_NOT_FOUND",
                        $"El requerimiento SSOMA {document.RequirementId} no existe o no pertenece a la empresa actual."));
                    continue;
                }

                if (!requirement.IsActive)
                {
                    documentErrors.Add(new GlobalErrorDetail(
                        "REQUIREMENT_INACTIVE",
                        $"El requerimiento SSOMA {document.RequirementId} se encuentra inactivo."));
                    continue;
                }

                if (requirement.ScopeId != dto.HomologationPersonnel.HomologationScopeId)
                {
                    documentErrors.Add(new GlobalErrorDetail(
                        "REQUIREMENT_SCOPE_MISMATCH",
                        $"El requerimiento SSOMA {document.RequirementId} no corresponde al alcance de la homologacion."));
                }
            }

            if (documentErrors.Count > 0)
                throw new AppValidationException(documentErrors);

            var calculationDto = new SsomaHomologationPersonnelCreateOrchestratedDto
            {
                HomologationPersonnel = dto.HomologationPersonnel,
                Documents = documents
            };

            var calculatedValidTo = _homologationCalculator.CalculateValidTo(calculationDto, operationEndDate);
            var headerValidFrom = dto.HomologationPersonnel.ValidFrom!.Value.Date;
            var headerValidTo = calculatedValidTo;
            var camoDocument = documents.FirstOrDefault(document => document.RequirementId == CamoRequirementId);

            if (dto.HomologationPersonnel.HomologationScopeId == ScopeOperation && camoDocument != null)
            {
                // CAMO solo gobierna fechas de cabecera en homologaciones por proyecto.
                headerValidFrom = camoDocument.IssueDate!.Value.Date;
                headerValidTo = camoDocument.ExpirationDate!.Value.Date;
            }

            var calculatedWorkerStatusId = _homologationCalculator.CalculateWorkerStatusId(calculationDto, headerValidTo);

            var personnelDto = new SsomaHomologationPersonnelCreateDto
            {
                HomologationScopeId = dto.HomologationPersonnel.HomologationScopeId,
                OperationsId = dto.HomologationPersonnel.OperationsId,
                WorkerId = dto.HomologationPersonnel.WorkerId,
                MedicalAptitudeId = dto.HomologationPersonnel.MedicalAptitudeId,
                ValidFrom = headerValidFrom,
                ValidTo = headerValidTo,
                WorkerStatusId = calculatedWorkerStatusId,
                SsomaApproved = dto.HomologationPersonnel.SsomaApproved,
                Notes = dto.HomologationPersonnel.Notes
            };

            var createdPersonnel = await _createSsomaHomologationPersonnel.ExecuteAsync(
                personnelDto,
                userId,
                businessId);

            if (createdPersonnel.Id is not > 0)
                return createdPersonnel;

            if (documents.Count == 0)
                return createdPersonnel;

            foreach (var document in documents)
            {
                var documentDto = new DocumentCreateDto
                {
                    HomologationPersonnelId = createdPersonnel.Id.Value,
                    RequirementId = document.RequirementId,
                    FileName = document.FileName,
                    FileUrl = document.FileUrl,
                    FilePath = document.FilePath,
                    IssueDate = document.IssueDate,
                    ExpirationDate = document.ExpirationDate,
                    ValidationStatusId = document.ValidationStatusId,
                    ReviewDate = document.ReviewDate,
                    Observation = document.Observation
                };

                await _createSsomaHomologationPersonnelDocument.ExecuteAsync(documentDto, userId, businessId);
            }

            return createdPersonnel;
        }
    }
}
