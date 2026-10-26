namespace Application.Constants
{
    public static class Permissions
    {
        // ========== CATALOG PERMISSIONS ==========
        public static class Catalog
        {
            public const string ViewCategories = "Catalog.Categories.View";
            public const string CreateCategory = "Catalog.Categories.Create";
            public const string EditCategory = "Catalog.Categories.Edit";
            public const string DeleteCategory = "Catalog.Categories.Delete";

            public const string ViewItems = "Catalog.Items.View";
            public const string CreateItem = "Catalog.Items.Create";
            public const string EditItem = "Catalog.Items.Edit";
            public const string DeleteItem = "Catalog.Items.Delete";
            public const string ManageStockLevels = "Catalog.Items.ManageStockLevels";
        }

        // ========== STORAGE PERMISSIONS ==========
        public static class Storage
        {
            public const string ViewWarehouses = "Storage.Warehouses.View";
            public const string CreateWarehouse = "Storage.Warehouses.Create";
            public const string EditWarehouse = "Storage.Warehouses.Edit";
            public const string DeleteWarehouse = "Storage.Warehouses.Delete";

            public const string ViewShelfLocations = "Storage.Shelves.View";
            public const string CreateShelfLocation = "Storage.Shelves.Create";
            public const string EditShelfLocation = "Storage.Shelves.Edit";
            public const string DeleteShelfLocation = "Storage.Shelves.Delete";

            public const string ViewInventory = "Storage.Inventory.View";
            public const string AdjustStock = "Storage.Inventory.Adjust";
            public const string TransferStock = "Storage.Inventory.Transfer";
            public const string CountStock = "Storage.Inventory.Count";
            public const string ViewStockLedger = "Storage.Inventory.ViewLedger";
        }

        // ========== PROPERTY PERMISSIONS ==========
        public static class Property
        {
            public const string ViewProperties = "Property.Properties.View";
            public const string CreateProperty = "Property.Properties.Create";
            public const string EditProperty = "Property.Properties.Edit";
            public const string DeleteProperty = "Property.Properties.Delete";
            public const string TransferProperty = "Property.Properties.Transfer";
            public const string AssignProperty = "Property.Properties.Assign";
            public const string DisposeProperty = "Property.Properties.Dispose";

            public const string ViewPropertyTypes = "Property.Types.View";
            public const string ManagePropertyTypes = "Property.Types.Manage";

            public const string ViewLocations = "Property.Locations.View";
            public const string ManageLocations = "Property.Locations.Manage";

            public const string ViewSafetyBoxes = "Property.SafetyBoxes.View";
            public const string ManageSafetyBoxes = "Property.SafetyBoxes.Manage";
        }

        // ========== REQUISITION PERMISSIONS ==========
        public static class Requisition
        {
            public const string ViewRequisitions = "Requisition.View";
            public const string CreateRequisition = "Requisition.Create";
            public const string EditRequisition = "Requisition.Edit";
            public const string DeleteRequisition = "Requisition.Delete";
            public const string ApproveRequisition = "Requisition.Approve";
            public const string RejectRequisition = "Requisition.Reject";
            public const string IssueRequisition = "Requisition.Issue";
            public const string CompleteRequisition = "Requisition.Complete";
            public const string CancelRequisition = "Requisition.Cancel";

            public const string ViewSIV = "Requisition.SIV.View";
            public const string CreateSIV = "Requisition.SIV.Create";
            public const string PrintSIV = "Requisition.SIV.Print";
        }

        // ========== RECEIVING PERMISSIONS ==========
        public static class Receiving
        {
            public const string ViewReceivingNotes = "Receiving.View";
            public const string CreateReceivingNote = "Receiving.Create";
            public const string EditReceivingNote = "Receiving.Edit";
            public const string DeleteReceivingNote = "Receiving.Delete";
            public const string ProcessReceivingNote = "Receiving.Process";

            public const string ViewSuppliers = "Receiving.Suppliers.View";
            public const string CreateSupplier = "Receiving.Suppliers.Create";
            public const string EditSupplier = "Receiving.Suppliers.Edit";
            public const string DeleteSupplier = "Receiving.Suppliers.Delete";

            public const string PerformInspection = "Receiving.Inspection.Perform";
            public const string ViewInspections = "Receiving.Inspection.View";
        }

        // ========== TRANSFER & RETURN PERMISSIONS ==========
        public static class TransferReturn
        {
            public const string ViewTransfers = "Transfer.View";
            public const string CreateTransfer = "Transfer.Create";
            public const string ApproveTransfer = "Transfer.Approve";

            public const string ViewReturns = "Return.View";
            public const string CreateReturn = "Return.Create";
            public const string ApproveReturn = "Return.Approve";
        }

        // ========== DISPOSAL PERMISSIONS ==========
        public static class Disposal
        {
            public const string ViewDisposals = "Disposal.View";
            public const string CreateDisposal = "Disposal.Create";
            public const string ApproveDisposal = "Disposal.Approve";
            public const string ProcessDisposal = "Disposal.Process";
        }

        // ========== USER MANAGEMENT PERMISSIONS ==========
        public static class UserManagement
        {
            public const string ViewUsers = "Users.View";
            public const string CreateUser = "Users.Create";
            public const string EditUser = "Users.Edit";
            public const string DeleteUser = "Users.Delete";
            public const string ActivateUser = "Users.Activate";
            public const string DeactivateUser = "Users.Deactivate";
            public const string ResetUserPassword = "Users.ResetPassword";

            public const string ViewRoles = "Users.Roles.View";
            public const string CreateRole = "Users.Roles.Create";
            public const string EditRole = "Users.Roles.Edit";
            public const string DeleteRole = "Users.Roles.Delete";
            public const string AssignRoles = "Users.Roles.Assign";

            public const string ViewPermissions = "Users.Permissions.View";
            public const string ManagePermissions = "Users.Permissions.Manage";

            public const string ViewEmployees = "Users.Employees.View";
            public const string ManageEmployees = "Users.Employees.Manage";
        }

        // ========== REPORT PERMISSIONS ==========
        public static class Reports
        {
            public const string ViewReports = "Reports.View";
            public const string ExportReports = "Reports.Export";
            public const string ScheduleReports = "Reports.Schedule";
            public const string ViewDashboard = "Reports.Dashboard.View";

            public const string ViewPropertyValuation = "Reports.PropertyValuation.View";
            public const string ViewRequisitionHistory = "Reports.RequisitionHistory.View";
            public const string ViewStockMovement = "Reports.StockMovement.View";
            public const string ViewInventoryStatus = "Reports.InventoryStatus.View";
            public const string ViewAuditTrail = "Reports.AuditTrail.View";
        }

        // ========== SYSTEM PERMISSIONS ==========
        public static class System
        {
            public const string ViewSystemSettings = "System.Settings.View";
            public const string EditSystemSettings = "System.Settings.Edit";
            public const string ManageBackup = "System.Backup.Manage";
            public const string ViewAuditLogs = "System.Audit.View";
            public const string ExportAuditLogs = "System.Audit.Export";
            public const string ManageNotifications = "System.Notifications.Manage";
            public const string ViewWorkflows = "System.Workflows.View";
            public const string ManageWorkflows = "System.Workflows.Manage";
            public const string ViewSystemHealth = "System.Health.View";
            public const string ManageSystemJobs = "System.Jobs.Manage";
        }

        // ========== WORKFLOW PERMISSIONS ==========
        public static class Workflow
        {
            public const string ViewWorkflows = "Workflow.View";
            public const string CreateWorkflow = "Workflow.Create";
            public const string EditWorkflow = "Workflow.Edit";
            public const string DeleteWorkflow = "Workflow.Delete";
            public const string ManageApprovers = "Workflow.Approvers.Manage";
        }

        // ========== AUDIT PERMISSIONS ==========
        public static class Audit
        {
            public const string ViewAuditTrail = "Audit.View";
            public const string ExportAuditTrail = "Audit.Export";
            public const string ManageAuditRetention = "Audit.Retention.Manage";
        }

        // ========== NOTIFICATION PERMISSIONS ==========
        public static class Notification
        {
            public const string ViewNotifications = "Notification.View";
            public const string SendNotifications = "Notification.Send";
            public const string ManageNotificationTemplates = "Notification.Templates.Manage";
            public const string ConfigureNotificationChannels = "Notification.Channels.Configure";
        }

        // ========== IMPORT/EXPORT PERMISSIONS ==========
        public static class ImportExport
        {
            public const string ImportData = "ImportExport.Import";
            public const string ExportData = "ImportExport.Export";
            public const string ViewImportHistory = "ImportExport.History.View";
        }

        // ========== API PERMISSIONS ==========
        public static class Api
        {
            public const string AccessApi = "Api.Access";
            public const string ManageApiKeys = "Api.Keys.Manage";
            public const string ViewApiLogs = "Api.Logs.View";
        }
    }
}