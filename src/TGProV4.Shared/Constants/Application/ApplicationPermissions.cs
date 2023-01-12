using System.ComponentModel;

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
        public const string View = "Permissions.Products.View";
        public const string Create = "Permissions.Products.Create";
        public const string Update = "Permissions.Products.Update";
        public const string Delete = "Permissions.Products.Delete";
        public const string Export = "Permissions.Products.Export";
        public const string Retrieve = "Permissions.Products.Retrieve";
    }
}
