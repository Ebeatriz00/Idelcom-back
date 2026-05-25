using Application.DTOs.SsomaHomologationPersonnel;
using Application.DTOs.SsomaHomologationPersonnelDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaHomologationPersonnel.services
{
    public interface ISsomaHomologationCalculator
    {
        DateTime CalculateValidTo(
            SsomaHomologationPersonnelCreateOrchestratedDto dto,
            DateTime? operationEndDate = null);

        long CalculateWorkerStatusId(
            SsomaHomologationPersonnelCreateOrchestratedDto dto,
            DateTime validTo);
    }
    public class SsomaHomologationCalculator : ISsomaHomologationCalculator
    {
        private const long ScopeGeneral = 1;
        private const long ScopeOperation = 2;

        private const int ValidationPending = 1;
        private const int ValidationValid = 2;
        private const int ValidationObserved = 3;
        private const int ValidationExpired = 4;

        // Ajusta estos IDs a tu catálogo real
        private const long WorkerStatusPending = 1;
        private const long WorkerStatusInProcess = 2;
        private const long WorkerStatusObserved = 3;
        private const long WorkerStatusApproved = 4;
        private const long WorkerStatusExpired = 5;
        private const long WorkerStatusRejected = 6;

        public DateTime CalculateValidTo(
            SsomaHomologationPersonnelCreateOrchestratedDto dto,
            DateTime? operationEndDate = null)
        {
            var personnel = dto.HomologationPersonnel;
            var documents = dto.Documents ?? new List<SsomaHomologationPersonnelDocumentCreateDto>();

            var validExpirationDates = documents
                .Where(d => d.ExpirationDate.HasValue)
                .Select(d => d.ExpirationDate!.Value.Date)
                .ToList();

            DateTime? minDocumentExpiration = validExpirationDates.Count > 0
                ? validExpirationDates.Min()
                : null;

            if (personnel.HomologationScopeId == ScopeGeneral)
            {
                // General: manda el menor vencimiento documental.
                // Si no hay docs con vencimiento, usamos ValidFrom como fallback.
                return minDocumentExpiration ?? personnel.ValidFrom!.Value.Date;
            }

            if (personnel.HomologationScopeId == ScopeOperation)
            {
                if (!operationEndDate.HasValue)
                    throw new InvalidOperationException("La homologación por operación requiere fecha fin de operación.");

                if (minDocumentExpiration.HasValue)
                    return MinDate(minDocumentExpiration.Value, operationEndDate.Value.Date);

                return operationEndDate.Value.Date;
            }

            throw new InvalidOperationException("El alcance de homologación no es válido.");
        }

        public long CalculateWorkerStatusId(
            SsomaHomologationPersonnelCreateOrchestratedDto dto,
            DateTime validTo)
        {
            var personnel = dto.HomologationPersonnel;
            var documents = dto.Documents ?? new List<SsomaHomologationPersonnelDocumentCreateDto>();

            var today = DateTime.Today;

            if (validTo.Date < today)
                return WorkerStatusExpired;

            if (documents.Count == 0)
                return WorkerStatusPending;

            if (documents.Any(d => d.ValidationStatusId == ValidationObserved))
                return WorkerStatusObserved;

            if (documents.Any(d => d.ValidationStatusId == ValidationExpired))
                return WorkerStatusExpired;

            if (documents.Any(d => d.ValidationStatusId == ValidationPending))
                return WorkerStatusInProcess;

            var allValid = documents.All(d => d.ValidationStatusId == ValidationValid);

            if (allValid && personnel.SsomaApproved)
                return WorkerStatusApproved;

            if (allValid && !personnel.SsomaApproved)
                return WorkerStatusInProcess;

            return WorkerStatusPending;
        }

        private static DateTime MinDate(DateTime a, DateTime b) => a <= b ? a : b;
    }
}
