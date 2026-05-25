using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Constants
{
    public static class TableNames
    {
        public const string Operations = "OPERATIONS";
        public const string OperationsProjectConfig = "OPERATIONS_PROJECT_CONFIG";
        public const string OperationsAttendance = "OPERATIONS_ATTENDANCE";
        public const string OperationsAttendanceSession = "OPERATIONS_ATTENDANCE_SESSION";
        public const string OperationsPersonnelAssignment = "OPERATIONS_PERSONNEL_ASSIGNMENT";
        public const string OperationsPersonnelMovement = "OPERATIONS_PERSONNEL_MOVEMENT";
        public const string OperationsWorkday = "OPERATIONS_WORKDAY";
        public const string OperationsWorkOrder = "OPERATIONS_WORK_ORDER";
        public const string OperationsWorkOrderResponsible = "OPERATIONS_WORK_ORDER_RESPONSIBLE";
        public const string OperationsWorkOrderActivity = "OPERATIONS_WORK_ORDER_ACTIVITY";
        public const string OperationsSquad = "OPERATIONS_SQUAD";
        public const string OperationsTeamSsoma = "OPERATIONS_TEAM_SSOMA";

        public const string ProductTypes = "PRODUCT_TYPES";
        public const string ProductLines = "PRODUCT_LINES";
        public const string Categories = "CATEGORIES";
        public const string Brands = "BRANDS";
        public const string Warehouses = "WAREHOUSES";
        public const string WarehousesMovement = "WAREHOUSE_MOVEMENT";
        public const string WarehousesMovementDetail = "WAREHOUSE_MOVEMENT_DETAIL";
        public const string InventoryStock = "INVENTORY_STOCK";
        public const string InventoryKardex = "INVENTORY_KARDEX";
        public const string Products = "PRODUCTS";
        public const string FileTrackingProducts = "FILE_TRACKING_PRODUCTS";
        public const string Suppliers = "SUPPLIERS";
        public const string PurchaseOrder = "PURCHASE_ORDER";
        public const string PurchaseOrderDetail = "PURCHASE_ORDER_DETAIL";
        public const string PurchaseOrderInvoice = "PURCHASE_ORDER_INVOICE";
        public const string PurchaseReceipt = "PURCHASE_RECEIPT";
        public const string PurchaseReceiptDetail = "PURCHASE_RECEIPT_DETAIL";
        public const string InventoryCount = "INVENTORY_COUNT";
        public const string InventoryCountDetail = "INVENTORY_COUNT_DETAIL";

        public const string SsomaProcess = "SSOMA_PROCESS";
        public const string SsomaRequirement = "SSOMA_REQUIREMENT";
        public const string SsomaOperationsRequirement = "SSOMA_OPERATIONS_REQUIREMENT";
        public const string SsomaHomologationPersonnel = "SSOMA_HOMOLOGATION_PERSONNEL";
        public const string SsomaHomologationPersonnelDocument = "SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT";

        public const string QuotationLogisticsSuggestion = "QUOTATION_LOGISTICS_SUGGESTION";
        public const string LogisticsRequest = "LOGISTICS_REQUEST";
        public const string LogisticsRequestDetail = "LOGISTICS_REQUEST_DETAIL";
    }
}
