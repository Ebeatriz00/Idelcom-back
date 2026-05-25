using Core.Commands.Logistic;
using Core.Filters.Logistic;
using Core.Interfaces.Logistic;
using Core.Projections.Logistic;
using Dapper;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Logistic
{
    public class QuotationLogisticsSuggestionRepository(IDapperHelper dapperHelper) : IQuotationLogisticsSuggestionRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<QuotationLogisticsSuggestionGenerateResult> GenerateAsync(GenerateQuotationLogisticsSuggestionCommand command)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.QuotationId,
                    command.QuotationVerId,
                    command.UserId
                });

                var result = await _dapperHelper.QueryFirstOrDefaultAsync<QuotationLogisticsSuggestionGenerateResult>(
                    "SP_WS_GENERATE_QUOTATION_LOGISTICS_SUGGESTIONS",
                    parameters);

                return result ?? throw new BusinessException("No se pudo generar sugerencias logisticas.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al generar sugerencias logisticas.", ex.Message);
            }
        }

        public async Task<IReadOnlyList<QuotationLogisticsSuggestionItem>> ListAsync(QuotationLogisticsSuggestionFilter filter)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    filter.BusinessId,
                    filter.QuotationId,
                    filter.QuotationVerId,
                    filter.ResourceTypeId,
                    filter.WorkOrderId,
                    filter.OnlySelected,
                    filter.Search
                });

                var items = await _dapperHelper.QueryAsync<QuotationLogisticsSuggestionItem>(
                    "SP_WS_LIST_QUOTATION_LOGISTICS_SUGGESTIONS",
                    parameters);

                return items.ToList();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al listar sugerencias logisticas.", ex.Message);
            }
        }

        public async Task<QuotationLogisticsSuggestionItem?> GetByIdAsync(long businessId, long suggestionId, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new { BusinessId = businessId, SuggestionId = suggestionId });

                return await _dapperHelper.QueryFirstOrDefaultAsync<QuotationLogisticsSuggestionItem>(
                    "SP_WS_GET_QUOTATION_LOGISTICS_SUGGESTION_BY_ID", parameters, transaction);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener la sugerencia logistica.", ex.Message);
            }
        }

        public async Task<BaseResponse> UpdateAsync(UpdateQuotationLogisticsSuggestionCommand command, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.SuggestionId,
                    command.BusinessId,
                    command.IsSelected,
                    command.ApprovedQuantity,
                    command.OfficeObservation,
                    command.WorkOrderId,
                    command.UserId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_QUOTATION_LOGISTICS_SUGGESTION", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo actualizar la sugerencia logistica.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar la sugerencia logistica.", ex.Message);
            }
        }

        public async Task<BaseResponseId> AddManualAsync(AddManualQuotationLogisticsSuggestionCommand command, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.QuotationId,
                    command.QuotationVerId,
                    command.WorkOrderId,
                    command.LogisticsResourceTypeId,
                    command.ProductsId,
                    command.Description,
                    command.SuggestedQuantity,
                    command.ApprovedQuantity,
                    command.OfficeObservation,
                    command.UserId
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_ADD_MANUAL_QUOTATION_LOGISTICS_SUGGESTION", parameters, transaction);

                var response = new BaseResponseId
                {
                    Status = parameters.Get<int>("@COutput"),
                    Message = parameters.Get<string>("@SOutput"),
                    Id = parameters.Get<long>("@Id")
                };

                if (response.Status != 1)
                    throw new BusinessException(response.Message ?? "No se pudo agregar la sugerencia manual.");

                return response;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al agregar la sugerencia manual.", ex.Message);
            }
        }

        public async Task<BaseResponse> AssignWorkOrderAsync(AssignWorkOrderQuotationLogisticsSuggestionCommand command, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.SuggestionId,
                    command.WorkOrderId,
                    command.UserId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_ASSIGN_WORK_ORDER_TO_QUOTATION_LOGISTICS_SUGGESTION", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo asignar la orden de trabajo a la sugerencia.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al asignar orden de trabajo a la sugerencia logistica.", ex.Message);
            }
        }

        public async Task<BaseResponse> DisableAsync(long businessId, long suggestionId, long userId, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    SuggestionId = suggestionId,
                    BusinessId = businessId,
                    UserId = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_DISABLE_QUOTATION_LOGISTICS_SUGGESTION", parameters, transaction);
                return BuildBaseResponse(parameters, "No se pudo desactivar la sugerencia logistica.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al desactivar la sugerencia logistica.", ex.Message);
            }
        }

        public async Task<CreateLogisticsRequestFromSelectedSuggestionsResult> CreateLogisticsRequestFromSelectedSuggestionsAsync(
            CreateLogisticsRequestFromSelectedSuggestionsCommand command,
            IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    command.BusinessId,
                    command.QuotationId,
                    command.QuotationVerId,
                    command.WorkOrderId,
                    command.SuggestionIdsCsv,
                    command.Observation,
                    command.OfficeObservation,
                    command.UserId
                })
                .WithOutputLong("@LogisticsRequestId")
                .WithOutputInt("@DetailCount")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_CREATE_LOGISTICS_REQUEST_FROM_SELECTED_SUGGESTIONS", parameters, transaction);

                var result = new CreateLogisticsRequestFromSelectedSuggestionsResult
                {
                    LogisticsRequestId = parameters.Get<long>("@LogisticsRequestId"),
                    DetailCount = parameters.Get<int>("@DetailCount"),
                    Status = parameters.Get<int>("@COutput"),
                    Message = parameters.Get<string>("@SOutput")
                };

                if (result.Status != 1)
                    throw new BusinessException(result.Message ?? "No se pudo crear la solicitud logistica desde sugerencias.");

                return result;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al crear solicitud logistica desde sugerencias.", ex.Message);
            }
        }

        private static BaseResponse BuildBaseResponse(DynamicParameters parameters, string fallbackMessage)
        {
            var response = new BaseResponse
            {
                Status = parameters.Get<int>("@COutput"),
                Message = parameters.Get<string>("@SOutput")
            };

            if (response.Status != 1)
                throw new BusinessException(response.Message ?? fallbackMessage);

            return response;
        }
    }
}
