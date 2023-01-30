using System.Reflection;
using TGProV4.Shared.Constants.Application;

namespace TGProV4.Shared.Helpers;

public static class ConstantHelpers
{
    public static IEnumerable<Tuple<string, string, IEnumerable<string?>>> GetApplicationPermissions()
    {
        var permissionInfo = new List<Tuple<string, string, IEnumerable<string?>>>();

        var displayNameAttribute =
            (DisplayNameAttribute[]) Attribute.GetCustomAttributes(typeof(ApplicationPermissions),
                typeof(DisplayNameAttribute));

        var descriptionAttributes =
            (DescriptionAttribute[]) Attribute.GetCustomAttributes(typeof(ApplicationPermissions),
                typeof(DescriptionAttribute));

        for (var i = 0; i < displayNameAttribute.Length; i++)
        {
            var permissions = typeof(ApplicationPermissions).GetNestedTypes()
                                                            .SelectMany(c => c.GetFields(BindingFlags.Public |
                                                                 BindingFlags.Static |
                                                                 BindingFlags.FlattenHierarchy))
                                                            .Select(field => field.GetValue(null)?.ToString())
                                                            .Where(fieldValue
                                                                 => !string.IsNullOrEmpty(fieldValue) &&
                                                                    fieldValue.Contains(displayNameAttribute[i]
                                                                       .DisplayName))
                                                            .ToList();

            permissionInfo.Add(new Tuple<string, string, IEnumerable<string?>>(displayNameAttribute[i]
               .DisplayName, descriptionAttributes[i].Description, permissions));
        }

        return permissionInfo;
    }
}
