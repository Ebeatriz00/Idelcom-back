using Core.Interfaces.Operations;
using SharedKernel;

namespace Application.UseCases.OperationsTeamSsoma
{
    public class DeleteSsomaAssignment(IOperationsTeamSsomaRepository repository)
    {
        private readonly IOperationsTeamSsomaRepository _repository = repository;

        public async Task<GlobalResponse> ExecuteAsync(long operationsTeamSsomaId, long userId, long businessId)
        {
            if (operationsTeamSsomaId <= 0)
                throw new ArgumentException("El ID de la asignación SSOMA es inválido.");

            // Aquí se podrían añadir reglas de negocio adicionales, por ejemplo:
            // - Verificar si tiene asistencias registradas
            // - Verificar si tiene reportes de seguridad asociados
            
            return await _repository.DeleteAsync(operationsTeamSsomaId, userId, businessId);
        }
    }
}
