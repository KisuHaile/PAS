namespace PAS.API.Constants;

public static class Permissions
{
    public static class Dashboard
    {
        public const string View = "Permissions.Dashboard.View";
    }

    public static class Categories
    {
        public const string View = "Permissions.Categories.View";
        public const string Create = "Permissions.Categories.Create";
        public const string Edit = "Permissions.Categories.Edit";
        public const string Delete = "Permissions.Categories.Delete";
    }

    public static class Items
    {
        public const string View = "Permissions.Items.View";
        public const string Create = "Permissions.Items.Create";
        public const string Edit = "Permissions.Items.Edit";
        public const string Delete = "Permissions.Items.Delete";
    }

    public static class Properties
    {
        public const string View = "Permissions.Properties.View";
        public const string Create = "Permissions.Properties.Create";
        public const string Edit = "Permissions.Properties.Edit";
        public const string Delete = "Permissions.Properties.Delete";
        public const string Transfer = "Permissions.Properties.Transfer";
    }

    public static class Requisitions
    {
        public const string View = "Permissions.Requisitions.View";
        public const string Create = "Permissions.Requisitions.Create";
        public const string Approve = "Permissions.Requisitions.Approve";
        public const string Issue = "Permissions.Requisitions.Issue";
    }

    public static class Notifications
    {
        public const string View = "Permissions.Notifications.View";
        public const string MarkAsRead = "Permissions.Notifications.MarkAsRead";
    }
}