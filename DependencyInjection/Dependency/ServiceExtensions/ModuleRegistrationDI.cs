using Core.Interfaces.Abstractions;
using Core.Interfaces.Services;
using DependencyInjection.Dependency.Modules;
using DependencyInjection.Dependency.Modules.AppAuth;
using DependencyInjection.Dependency.Modules.Audit;
using DependencyInjection.Dependency.Modules.Logistic;
using DependencyInjection.Dependency.Modules.Operations;
using DependencyInjection.Dependency.Modules.OperationsSquad;
using DependencyInjection.Dependency.Modules.Ssoma;
using Infrastructure.Dependency.Modules;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ITokenService = Core.Interfaces.Services.ITokenService;

namespace DependencyInjection.Dependency.ServiceExtensions
{
    public static class ModuleRegistrationDI
    {
        public static IServiceCollection AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGeneralBusinessModules(configuration);
            services.AddOperationsModules();
            services.AddSecurityModules();
            services.AddPersistenceDI();

            return services;
        }

        public static IServiceCollection AddGeneralBusinessModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuditInjection();
            services.AddDocumentTypeServices();
            services.AddCurrencyServices();
            services.AddProfilesServices();
            services.AddUsersServices();
            services.AddAuthServices();
            services.AddAppAuthServices();
            services.AddBusinessServices();
            services.AddModulesPermissionsServices();
            services.AddProfilesPermissionsServices();
            services.AddPaymentTypeServices();
            services.AddParentModulesServices();
            services.AddModulesServices();
            services.AddPermissionsServices();
            services.AddAreaServices();
            services.AddWorkerServices();
            services.AddJobTitleServices();
            services.AddLocationServices();
            services.AddBankServices();
            services.AddTaxAffTypeServices();
            services.AddTaxesServices();
            services.AddPurchaseOrderStatusServices();
            services.AddPurchaseOrderDetailStatusServices();
            services.AddPurchaseReceiptStatusServices();
            services.AddSupplierInvoiceStatusServices();
            services.AddPayableStatusServices();
            services.AddAccountPlanServices();
            services.AddAccountTypeServices();
            services.AddAccountLevelServices();
            services.AddTypeAnalysisServices();
            services.AddAuxiliarTypeServices();
            services.AddLeadsSourcesServices();
            services.AddLeadsStatusServices();
            services.AddUsersPreferencesServices();
            services.AddStateOpportunityServices();
            services.AddBusinessLineServices();
            services.AddProcessTypeServices();
            services.AddClientsServices();
            services.AddSectorServices();
            services.AddReasonRejectionServices();
            services.AddOpportunitiesServices();
            services.AddFileTrackingServices();
            services.AddActivityStateServices();
            services.AddActivityServices();
            services.AddActivityTypeServices();
            services.AddCommentServices();
            services.AddCommercialParametersServices();
            services.AddNotificationServices();
            services.AddNotificationsEmailServices(configuration);
            services.AddTypeSuppliersServices();
            services.AddSalesQuotationServices();
            services.AddNegotiationStagesServices();
            services.AddHiringServices();
            services.AddSsomaAsiggnmanetTypeServices();
            services.AddWorkerStatusServices();
            services.AddMedicalAptitudeServices();
            services.AddSsomaMovementTypeServices();
            services.AddSsomaDocumentTypeServices();
            services.AddSsomaProcessInjection();
            services.AddSsomaRequirementInjection();
            services.AddSsomaOperationsRequirementInjection();
            services.AddSsomaHomologationPersonnelInjection();
            services.AddSsomaHomologationPersonnelDocumentInjection();
            services.AddOrdersServices();
            services.AddDashboardPreSalesServices();
            services.AddLicStatusServices();
            services.AddObservationsServices();
            services.AddClientsActivityServices();
            services.AddSubTasksServices();
            services.AddDashboardServices();
            services.AddOpporViabilityServices();
            services.AddFileTrackingProductsServices();
            services.AddProductsServices();
            services.AddViabilityServices();
            services.AddProjectTeamServices();
            services.AddPreSaleProyectsServices();
            services.AddStatePreSaleServices();
            services.AddPeriodsServices();
            services.AddExercisesServices();
            services.AddPriorityStateServices();
            services.AddPaymentMethodServices();
            services.AddPMVisServices();
            services.AddPMConditionServices();
            services.AddSuppliersServices();
            services.AddSupplierGroupsServices();
            services.AddTasksServices();
            services.AddStateTaskServices();
            services.AddAccountServices();
            services.AddBoxesServices();
            services.AddContactsCrmServices();
            services.AddMovementTypesServices();
            services.AddMovSunatServices();
            services.AddMovVisServices();
            services.AddMovPerServices();
            services.AddMovClasServices();
            services.AddMovOperServices();
            services.AddWarehousesServices();
            services.AddInventoryStockServices();
            services.AddProductTypesServices();
            services.AddBrandsServices();
            services.AddProductLinesServices();
            services.AddCategoriesServices();
            services.AddLeadsQualificationsServices();
            services.AddContactTypeServices();
            services.AddConceptsServices();
            services.AddConceptTypeServices();
            services.AddConceptGroupsServices();
            services.AddCostCentersServices();
            services.AddSeriesServices();
            services.AddExchangeRateServices();
            services.AddUomServices();
            services.AddApiPeruServices(configuration);
            services.AddSupportStateInjection();
            services.AddFileStorageServices(configuration);

            return services;
        }

        public static IServiceCollection AddOperationsModules(this IServiceCollection services)
        {
            services.AddAssignmentStatusInjection();
            services.AddAttendanceStatusInjection();
            services.AddMovementStatusInjection();
            services.AddOperationsInjection();
            services.AddOperationsStatusInjection();
            services.AddOperationsTeamSsomaInjection();
            services.AddOperationsProjectConfigInjection();
            services.AddOperationsWorkOrderInjection();
            services.AddOperationsSquadServices();
            services.AddOperationsSupervisorServices();
            services.AddOperationsWorkOrderResponsibleInjection();
            services.AddOperationsWorkOrderStatusInkection();
            services.AddOperationsPersonnelAssignmentServices();
            services.AddOperationPersonnelMovementServices();
            services.AddWorkDayStatusInjection();
            services.AddMeasurementUnitInjection();
            services.AddActivityComplexityInjection();
            services.AddSsomaRoleInjection();
            services.AddOperationsWorkOrderActivityInjection();
            services.AddOperationsWorkOrderProgressInjection();
            services.AddAppAttendanceInjection();
            services.AddSupportServices();

            return services;
        }

        public static IServiceCollection AddSecurityModules(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddSingleton<ITokenBlacklist, TokenBlacklist>();
            services.AddScoped<ILoginAttemptService, LoginAttemptService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddSingleton<ILinkTokenService, LinkTokenService>();
            services.AddSingleton<IUserSessionService, UserSessionService>();

            return services;
        }
    }
}
