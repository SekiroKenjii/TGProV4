namespace TGProV4.Shared.Constants.Application;

public static class ApplicationPermissions
{
    [DisplayName("Brands")]
    [Description("Brands Permissions")]
    public static class Brands
    {
        public const string Create = "Permissions.Brands.Create";
        public const string Read = "Permissions.Brands.Read";
        public const string Update = "Permissions.Brands.Update";
        public const string Delete = "Permissions.Brands.Delete";
        public const string Export = "Permissions.Brands.Export";
        public const string Retrieve = "Permissions.Brands.Retrieve";
    }

    [DisplayName("Products")]
    [Description("Products Permissions")]
    public static class Products
    {
        public const string Create = "Permissions.Products.Create";
        public const string Read = "Permissions.Products.Read";
        public const string Update = "Permissions.Products.Update";
        public const string Delete = "Permissions.Products.Delete";
        public const string Export = "Permissions.Products.Export";
        public const string Retrieve = "Permissions.Products.Retrieve";
    }

    [DisplayName("Users")]
    [Description("Users Permissions")]
    public static class Users
    {
        public const string Create = "Permissions.Users.Create";
        public const string Read = "Permissions.Users.Read";
        public const string Update = "Permissions.Users.Update";
        public const string Delete = "Permissions.Users.Delete";
        public const string Export = "Permissions.Users.Export";
        public const string Retrieve = "Permissions.Users.Retrieve";
    }

    [DisplayName("Roles")]
    [Description("Roles Permissions")]
    public static class Roles
    {
        public const string Create = "Permissions.Roles.Create";
        public const string Read = "Permissions.Roles.Read";
        public const string Update = "Permissions.Roles.Update";
        public const string Delete = "Permissions.Roles.Delete";
        public const string Export = "Permissions.Roles.Export";
        public const string Retrieve = "Permissions.Roles.Retrieve";
    }

    [DisplayName("Role Claims")]
    [Description("Role Claims Permissions")]
    public static class RoleClaims
    {
        public const string Create = "Permissions.RoleClaims.Create";
        public const string Read = "Permissions.RoleClaims.Read";
        public const string Update = "Permissions.RoleClaims.Update";
        public const string Delete = "Permissions.RoleClaims.Delete";
        public const string Export = "Permissions.RoleClaims.Export";
        public const string Retrieve = "Permissions.RoleClaims.Retrieve";
    }

    [DisplayName("Communication")]
    [Description("Communication Permissions")]
    public static class Communications
    {
        public const string Chat = "Permissions.Communications.Chat";
        public const string Comment = "Permissions.Communications.Comment";
        public const string ReplyComment = "Permissions.Communications.ReplyComment";
    }

    [DisplayName("Dashboards")]
    [Description("Dashboards Permissions")]
    public static class Dashboards
    {
        public const string View = "Permissions.Dashboards.View";
    }

    [DisplayName("Hangfire")]
    [Description("Hangfire Permissions")]
    public static class Hangfires
    {
        public const string View = "Permissions.Hangfires.View";
    }

    [DisplayName("Audit Trails")]
    [Description("Audit Trails Permissions")]
    public static class AuditTrails
    {
        public const string View = "Permissions.AuditTrails.View";
        public const string Export = "Permissions.AuditTrails.Export";
        public const string Search = "Permissions.AuditTrails.Search";
    }
}
