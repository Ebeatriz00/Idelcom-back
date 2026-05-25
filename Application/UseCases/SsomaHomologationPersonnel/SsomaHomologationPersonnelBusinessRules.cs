using Core.Interfaces;
using Core.Interfaces.Operations;
using Infrastructure.Exceptions;

namespace Application.UseCases.SsomaHomologationPersonnel
{
    public class SsomaHomologationPersonnelBusinessRules(
        IOperationsRepository operationsRepository,
        IWorkerRepository workerRepository,
        IWorkerStatusRepository workerStatusRepository,
        IMedicalAptitudeRepository medicalAptitudeRepository)
    {
        private readonly IOperationsRepository _operationsRepository = operationsRepository;
        private readonly IWorkerRepository _workerRepository = workerRepository;
        private readonly IWorkerStatusRepository _workerStatusRepository = workerStatusRepository;
        private readonly IMedicalAptitudeRepository _medicalAptitudeRepository = medicalAptitudeRepository;

        public void Normalize(
            long? operationsId,
            DateTime validFrom,
            DateTime validTo,
            string? notes,
            out long? normalizedOperationsId,
            out DateTime normalizedValidFrom,
            out DateTime normalizedValidTo,
            out string normalizedNotes)
        {
            normalizedOperationsId = operationsId.HasValue && operationsId.Value > 0
                ? operationsId.Value
                : null;
            normalizedValidFrom = validFrom.Date;
            normalizedValidTo = validTo.Date;
            normalizedNotes = notes?.Trim() ?? string.Empty;
        }

        public async Task ValidateReferencesAsync(
            long businessId,
            long workerId,
            long workerStatusId,
            long medicalAptitudeId,
            long? operationsId)
        {
            if (operationsId.HasValue)
            {
                if (operationsId.Value <= 0)
                    throw new BusinessException("La operación indicada no es válida.");

                var operation = await _operationsRepository.GetByIdAsync(operationsId.Value);
                if (operation == null || operation.BusinessId != businessId)
                    throw new BusinessException("La operación no existe o no pertenece a la empresa actual.");
            }

            var worker = await _workerRepository.GetByIdAsync(workerId);
            if (worker == null || worker.BusinessId != businessId)
                throw new BusinessException("El trabajador no existe o no pertenece a la empresa actual.");

            var workerStatus = await _workerStatusRepository.GetHomologationByIdAsync(workerStatusId);
            if (workerStatus == null || workerStatus.BusinessId != businessId)
                throw new BusinessException("El estado del trabajador no existe.");

            if (workerStatus.Status != "1")
                throw new BusinessException("El estado del trabajador se encuentra inactivo.");

            if (medicalAptitudeId > 0)
            {
                var medicalAptitude = await _medicalAptitudeRepository.GetByIdAsync(medicalAptitudeId);
                if (medicalAptitude == null || medicalAptitude.BusinessId != businessId)
                    throw new BusinessException("La aptitud médica no existe o no pertenece a la empresa actual.");

                if (medicalAptitude.Status != "1")
                    throw new BusinessException("La aptitud médica se encuentra inactiva.");
            }
        }

        public async Task ValidateListFilterAsync(long businessId, long? operationsId)
        {
            if (!operationsId.HasValue)
                return;

            if (operationsId.Value <= 0)
                throw new BusinessException("La operación indicada no es válida.");

            var operation = await _operationsRepository.GetByIdAsync(operationsId.Value);
            if (operation == null || operation.BusinessId != businessId)
                throw new BusinessException("La operación no existe o no pertenece a la empresa actual.");
        }
    }
}
