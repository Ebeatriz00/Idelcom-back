using Application.DTOs.Warehouses;
using Application.DTOs.WarehousesMovement;
using Application.DTOs.PurchaseOrder;
using Application.DTOs.PurchaseReceipt;
using Application.DTOs.InventoryCount;
using Application.DTOs.QuotationLogisticsSuggestion;
using Application.DTOs.LogisticsRequest;
using Application.UseCases.Warehouses;
using Application.UseCases.WarehousesMovement;
using Application.UseCases.PurchaseOrder;
using Application.UseCases.PurchaseReceipt;
using Application.UseCases.InventoryCount;
using Application.UseCases.QuotationLogisticsSuggestion;
using Application.UseCases.LogisticsRequest;
using Application.Validators.Warehouses;
using Application.Validators.WarehousesMovement;
using Application.Validators.PurchaseOrder;
using Application.Validators.PurchaseReceipt;
using Application.Validators.InventoryCount;
using Application.Validators.QuotationLogisticsSuggestion;
using Application.Validators.LogisticsRequest;
using Core.Interfaces.Logistic;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Logistic;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Dependency.Modules
{
    public static class WarehousesInjection
    {
        public static IServiceCollection AddWarehousesServices(this IServiceCollection services)
        {
            services.AddScoped<CreateWarehouses>();
            services.AddScoped<GetAllWarehouses>();
            services.AddScoped<GetWarehousesById>();
            services.AddScoped<UpdateWarehouses>();
            services.AddScoped<PatchWarehousesStatus>();
            services.AddScoped<GetSelectWarehouses>();
            services.AddScoped<CreateWarehousesMovement>();
            services.AddScoped<WarehousesMovementBusinessRules>();
            services.AddScoped<WarehousesMovementStockService>();
            services.AddScoped<WarehouseMovementQueryService>();
            services.AddScoped<RegisterPurchaseOrder>();
            services.AddScoped<ListPurchaseOrder>();
            services.AddScoped<GetPurchaseOrderById>();
            services.AddScoped<UpdatePurchaseOrder>();
            services.AddScoped<ApprovePurchaseOrder>();
            services.AddScoped<CancelPurchaseOrder>();
            services.AddScoped<AttachPurchaseOrderInvoice>();
            services.AddScoped<CreatePurchaseOrderFromInvoice>();
            services.AddScoped<GeneratePurchaseOrderPdf>();
            services.AddScoped<SendPurchaseOrderForApproval>();
            services.AddScoped<CreatePurchaseReceiptUseCase>();
            services.AddScoped<ListPurchaseReceiptUseCase>();
            services.AddScoped<GetPurchaseReceiptByIdUseCase>();
            services.AddScoped<VoidPurchaseReceiptUseCase>();
            services.AddScoped<RegularizePurchaseReceiptWithPurchaseOrderUseCase>();
            services.AddScoped<CreateInventoryCountUseCase>();
            services.AddScoped<StartInventoryCountUseCase>();
            services.AddScoped<UpdateInventoryCountDetailsUseCase>();
            services.AddScoped<CloseInventoryCountUseCase>();
            services.AddScoped<GenerateInventoryCountAdjustmentsUseCase>();
            services.AddScoped<CancelInventoryCountUseCase>();
            services.AddScoped<ListInventoryCountUseCase>();
            services.AddScoped<GetInventoryCountByIdUseCase>();
            services.AddScoped<GenerateQuotationLogisticsSuggestionsUseCase>();
            services.AddScoped<ListQuotationLogisticsSuggestionsUseCase>();
            services.AddScoped<UpdateQuotationLogisticsSuggestionUseCase>();
            services.AddScoped<AddManualQuotationLogisticsSuggestionUseCase>();
            services.AddScoped<AssignWorkOrderQuotationLogisticsSuggestionUseCase>();
            services.AddScoped<DisableQuotationLogisticsSuggestionUseCase>();
            services.AddScoped<CreateLogisticsRequestFromSelectedSuggestionsUseCase>();

            services.AddTransient<IValidator<WarehousesCreateDto>, WarehousesCreateValidator>();
            services.AddTransient<IValidator<WarehousesUpdateDto>, WarehousesUpdateValidator>();
            services.AddTransient<IValidator<WarehousesStatusToggleDto>, WarehousesStatusToggleValidator>();
            services.AddTransient<IValidator<WarehousesMovementCreateDto>, WarehousesMovementCreateValidator>();
            services.AddTransient<IValidator<PurchaseOrderCreateDto>, PurchaseOrderCreateValidator>();
            services.AddTransient<IValidator<PurchaseOrderUpdateDto>, PurchaseOrderUpdateValidator>();
            services.AddTransient<IValidator<PurchaseOrderApproveDto>, PurchaseOrderApproveValidator>();
            services.AddTransient<IValidator<PurchaseOrderCancelDto>, PurchaseOrderCancelValidator>();
            services.AddTransient<IValidator<PurchaseOrderListFilterDto>, PurchaseOrderListFilterValidator>();
            services.AddTransient<IValidator<PurchaseOrderAttachInvoiceDto>, PurchaseOrderAttachInvoiceValidator>();
            services.AddTransient<IValidator<PurchaseOrderCreateFromInvoiceDto>, PurchaseOrderCreateFromInvoiceValidator>();
            services.AddTransient<IValidator<PurchaseOrderSendForApprovalDto>, PurchaseOrderSendForApprovalValidator>();
            services.AddTransient<IValidator<PurchaseReceiptCreateDto>, PurchaseReceiptCreateValidator>();
            services.AddTransient<IValidator<PurchaseReceiptRegularizeDto>, PurchaseReceiptRegularizeValidator>();
            services.AddTransient<IValidator<PurchaseReceiptListFilterDto>, PurchaseReceiptListFilterValidator>();
            services.AddTransient<IValidator<InventoryCountCreateDto>, InventoryCountCreateValidator>();
            services.AddTransient<IValidator<InventoryCountUpdateDetailsDto>, InventoryCountUpdateDetailsValidator>();
            services.AddTransient<IValidator<InventoryCountListFilterDto>, InventoryCountListFilterValidator>();
            services.AddTransient<IValidator<GenerateQuotationLogisticsSuggestionDto>, GenerateQuotationLogisticsSuggestionValidator>();
            services.AddTransient<IValidator<ListQuotationLogisticsSuggestionFilterDto>, ListQuotationLogisticsSuggestionValidator>();
            services.AddTransient<IValidator<UpdateQuotationLogisticsSuggestionDto>, UpdateQuotationLogisticsSuggestionValidator>();
            services.AddTransient<IValidator<AddManualQuotationLogisticsSuggestionDto>, AddManualQuotationLogisticsSuggestionValidator>();
            services.AddTransient<IValidator<AssignWorkOrderQuotationLogisticsSuggestionDto>, AssignWorkOrderQuotationLogisticsSuggestionValidator>();
            services.AddTransient<IValidator<CreateLogisticsRequestFromSelectedSuggestionsDto>, CreateLogisticsRequestFromSelectedSuggestionsValidator>();

            services.AddScoped<IWarehousesRepository, WarehousesRepository>();
            services.AddScoped<IWarehousesMovement, WarehousesMovementRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IPurchaseReceiptRepository, PurchaseReceiptRepository>();
            services.AddScoped<IInventoryCountRepository, InventoryCountRepository>();
            services.AddScoped<IQuotationLogisticsSuggestionRepository, QuotationLogisticsSuggestionRepository>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}
