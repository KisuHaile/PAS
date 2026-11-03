namespace Application.Constants
{
    public static class SystemRoles
    {
        // ========== SYSTEM ROLES ==========
        public const string SuperAdmin = "Super Admin";
        public const string Admin = "Admin";
        public const string StoreManager = "Store Manager";
        public const string StoreOfficer = "Store Officer";
        public const string PropertyManager = "Property Manager";
        public const string PropertyOfficer = "Property Officer";
        public const string RequisitionOfficer = "Requisition Officer";
        public const string Approver = "Approver";
        public const string Auditor = "Auditor";
        public const string Inspector = "Inspector";
        public const string Manager = "Manager";
        public const string Staff = "Staff";
        public const string Viewer = "Viewer";

        // ========== GET ALL ROLES ==========
        public static List<string> GetAll()
        {
            return new List<string>
            {
                SuperAdmin, Admin, StoreManager, StoreOfficer, PropertyManager,
                PropertyOfficer, RequisitionOfficer, Approver, Auditor, Staff, Viewer
            };
        }

        // ========== CHECK IF ROLE EXISTS ==========
        public static bool IsValid(string role)
        {
            return GetAll().Contains(role);
        }

        // ========== GET ADMIN ROLES ==========
        public static List<string> GetAdminRoles()
        {
            return new List<string> { SuperAdmin, Admin };
        }

        // ========== GET MANAGER ROLES ==========
        public static List<string> GetManagerRoles()
        {
            return new List<string> { StoreManager, PropertyManager };
        }

        // ========== GET OFFICER ROLES ==========
        public static List<string> GetOfficerRoles()
        {
            return new List<string> { StoreOfficer, PropertyOfficer, RequisitionOfficer };
        }

        // ========== GET APPROVER ROLES ==========
        public static List<string> GetApproverRoles()
        {
            return new List<string> { Admin, StoreManager, PropertyManager, Approver };
        }

        // ========== DEFAULT ROLE PERMISSIONS ==========
        public static class DefaultPermissions
        {
            public static Dictionary<string, List<string>> Get()
            {
                return new Dictionary<string, List<string>>
                {
                    [SuperAdmin] = new List<string>
                    {
                        Permissions.Catalog.ViewCategories, Permissions.Catalog.CreateCategory,
                        Permissions.Catalog.EditCategory, Permissions.Catalog.DeleteCategory,
                        Permissions.Catalog.ViewItems, Permissions.Catalog.CreateItem,
                        Permissions.Catalog.EditItem, Permissions.Catalog.DeleteItem,
                        Permissions.Storage.ViewWarehouses, Permissions.Storage.CreateWarehouse,
                        Permissions.Storage.EditWarehouse, Permissions.Storage.DeleteWarehouse,
                        Permissions.Storage.ViewInventory, Permissions.Storage.AdjustStock,
                        Permissions.Property.ViewProperties, Permissions.Property.CreateProperty,
                        Permissions.Property.EditProperty, Permissions.Property.DeleteProperty,
                        Permissions.Property.TransferProperty, Permissions.Property.AssignProperty,
                        Permissions.Requisition.ViewRequisitions, Permissions.Requisition.CreateRequisition,
                        Permissions.Requisition.ApproveRequisition, Permissions.Requisition.IssueRequisition,
                        Permissions.UserManagement.ViewUsers, Permissions.UserManagement.CreateUser,
                        Permissions.UserManagement.EditUser, Permissions.UserManagement.DeleteUser,
                        Permissions.Reports.ViewReports, Permissions.Reports.ExportReports,
                        Permissions.System.ViewSystemSettings, Permissions.System.EditSystemSettings,
                        Permissions.Audit.ViewAuditTrail
                    },

                    [Admin] = new List<string>
                    {
                        Permissions.Catalog.ViewCategories, Permissions.Catalog.CreateCategory, Permissions.Catalog.EditCategory,
                        Permissions.Catalog.ViewItems, Permissions.Catalog.CreateItem, Permissions.Catalog.EditItem,
                        Permissions.Storage.ViewWarehouses, Permissions.Storage.CreateWarehouse, Permissions.Storage.EditWarehouse,
                        Permissions.Storage.ViewInventory, Permissions.Storage.AdjustStock,
                        Permissions.Property.ViewProperties, Permissions.Property.CreateProperty, Permissions.Property.EditProperty,
                        Permissions.Property.TransferProperty, Permissions.Property.AssignProperty,
                        Permissions.Requisition.ViewRequisitions, Permissions.Requisition.CreateRequisition,
                        Permissions.Requisition.ApproveRequisition, Permissions.Requisition.IssueRequisition,
                        Permissions.UserManagement.ViewUsers, Permissions.UserManagement.CreateUser,
                        Permissions.UserManagement.EditUser, Permissions.UserManagement.ViewRoles,
                        Permissions.Reports.ViewReports, Permissions.Reports.ExportReports
                    },

                    [StoreManager] = new List<string>
                    {
                        Permissions.Catalog.ViewCategories, Permissions.Catalog.ViewItems,
                        Permissions.Storage.ViewWarehouses, Permissions.Storage.CreateWarehouse, Permissions.Storage.EditWarehouse,
                        Permissions.Storage.ViewInventory, Permissions.Storage.AdjustStock, Permissions.Storage.TransferStock,
                        Permissions.Requisition.ViewRequisitions, Permissions.Requisition.ApproveRequisition,
                        Permissions.Requisition.IssueRequisition,
                        Permissions.Receiving.ViewReceivingNotes, Permissions.Receiving.CreateReceivingNote,
                        Permissions.Reports.ViewReports
                    },

                    [StoreOfficer] = new List<string>
                    {
                        Permissions.Catalog.ViewCategories, Permissions.Catalog.ViewItems,
                        Permissions.Storage.ViewWarehouses, Permissions.Storage.ViewShelfLocations,
                        Permissions.Storage.ViewInventory, Permissions.Storage.CountStock,
                        Permissions.Requisition.ViewRequisitions, Permissions.Requisition.IssueRequisition,
                        Permissions.Receiving.ViewReceivingNotes, Permissions.Receiving.CreateReceivingNote,
                        Permissions.Reports.ViewReports
                    },

                    [PropertyManager] = new List<string>
                    {
                        Permissions.Catalog.ViewCategories, Permissions.Catalog.ViewItems,
                        Permissions.Property.ViewProperties, Permissions.Property.CreateProperty,
                        Permissions.Property.EditProperty, Permissions.Property.TransferProperty,
                        Permissions.Property.AssignProperty, Permissions.Property.DisposeProperty,
                        Permissions.Property.ViewPropertyTypes, Permissions.Property.ManagePropertyTypes,
                        Permissions.Property.ViewLocations, Permissions.Property.ManageLocations,
                        Permissions.Requisition.ViewRequisitions, Permissions.Requisition.ApproveRequisition,
                        Permissions.Reports.ViewReports
                    },

                    [PropertyOfficer] = new List<string>
                    {
                        Permissions.Catalog.ViewCategories, Permissions.Catalog.ViewItems,
                        Permissions.Property.ViewProperties, Permissions.Property.CreateProperty,
                        Permissions.Property.EditProperty, Permissions.Property.ViewPropertyTypes,
                        Permissions.Property.ViewLocations,
                        Permissions.Reports.ViewReports
                    },

                    [RequisitionOfficer] = new List<string>
                    {
                        Permissions.Catalog.ViewItems,
                        Permissions.Requisition.ViewRequisitions, Permissions.Requisition.CreateRequisition,
                        Permissions.Requisition.EditRequisition, Permissions.Requisition.ViewSIV,
                        Permissions.Reports.ViewReports
                    },

                    [Approver] = new List<string>
                    {
                        Permissions.Requisition.ViewRequisitions, Permissions.Requisition.ApproveRequisition,
                        Permissions.Requisition.RejectRequisition,
                        Permissions.TransferReturn.ViewTransfers, Permissions.TransferReturn.ApproveTransfer,
                        Permissions.Reports.ViewReports
                    },

                    [Auditor] = new List<string>
                    {
                        Permissions.Reports.ViewReports, Permissions.Reports.ExportReports,
                        Permissions.Audit.ViewAuditTrail,
                        Permissions.Storage.ViewInventory,
                        Permissions.Property.ViewProperties
                    },

                    [Staff] = new List<string>
                    {
                        Permissions.Catalog.ViewItems,
                        Permissions.Property.ViewProperties,
                        Permissions.Requisition.ViewRequisitions, Permissions.Requisition.CreateRequisition,
                        Permissions.Reports.ViewDashboard
                    },

                    [Viewer] = new List<string>
                    {
                        Permissions.Reports.ViewDashboard,
                        Permissions.Catalog.ViewItems
                    }
                };
            }

        public static class RolePermissions
        {
            public static readonly string[] AdminPermissions =
            {
                Permissions.System.ViewSystemSettings,
                Permissions.Reports.ViewReports
            };

            public static readonly string[] StoreOfficerPermissions =
            {
                Permissions.Storage.ViewInventory,
                Permissions.Requisition.ViewRequisitions
            };

            public static readonly string[] StaffPermissions =
            {
                Permissions.Requisition.CreateRequisition,
                Permissions.Reports.ViewDashboard
            };

            public static readonly string[] InspectorPermissions =
            {
                Permissions.Receiving.PerformInspection,
                Permissions.Receiving.ViewInspections
            };

            public static readonly string[] ApproverPermissions =
            {
                Permissions.Requisition.ApproveRequisition,
                Permissions.TransferReturn.ApproveTransfer
            };

            public static readonly string[] ManagerPermissions =
            {
                Permissions.Reports.ViewReports,
                Permissions.Workflow.ViewWorkflows
            };
        }
        }

        public static class RolePermissions
        {
            public static readonly string[] AdminPermissions = { Permissions.System.ViewSystemSettings, Permissions.Reports.ViewReports };
            public static readonly string[] StoreOfficerPermissions = { Permissions.Storage.ViewInventory, Permissions.Requisition.ViewRequisitions };
            public static readonly string[] StaffPermissions = { Permissions.Requisition.CreateRequisition, Permissions.Reports.ViewDashboard };
            public static readonly string[] InspectorPermissions = { Permissions.Receiving.PerformInspection, Permissions.Receiving.ViewInspections };
            public static readonly string[] ApproverPermissions = { Permissions.Requisition.ApproveRequisition, Permissions.TransferReturn.ApproveTransfer };
            public static readonly string[] ManagerPermissions = { Permissions.Reports.ViewReports, Permissions.Workflow.ViewWorkflows };
        }
    }
}