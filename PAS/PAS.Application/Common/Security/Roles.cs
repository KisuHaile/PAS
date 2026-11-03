namespace Application.Common.Security
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string StoreOfficer = "Store Officer";
        public const string Staff = "Staff";
        public const string Viewer = "Viewer";

        public static readonly List<string> AllRoles = new()
        {
            Admin,
            StoreOfficer,
            Staff,
            Viewer
        };
    }

    public static class Policies
    {
        public const string CanManageProperties = "CanManageProperties";
        public const string CanManageUsers = "CanManageUsers";
        public const string CanApproveRequisitions = "CanApproveRequisitions";
        public const string CanViewReports = "CanViewReports";
    }
}