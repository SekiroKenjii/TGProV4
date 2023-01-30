namespace TGProV4.Shared.Helpers;

public static class ConstantHelpers
{
    public static IEnumerable<Tuple<string, string, IEnumerable<string?>>> GetApplicationPermissions()
    {
        var permissionInfo = new List<Tuple<string, string, IEnumerable<string?>>>();

        var displayNameAttribute =
            typeof(ApplicationPermissions)
               .GetNestedTypes()
               .Select(type => (DisplayNameAttribute) type.GetCustomAttribute(typeof(DisplayNameAttribute), false)!)
               .ToArray();

        var descriptionAttributes =
            typeof(ApplicationPermissions)
               .GetNestedTypes()
               .Select(type => (DescriptionAttribute) type.GetCustomAttribute(typeof(DescriptionAttribute), false)!)
               .ToArray();

        for (var i = 0; i < displayNameAttribute.Length; i++)
        {
            var permissions = typeof(ApplicationPermissions)
                             .GetNestedTypes()
                             .SelectMany(t => t.GetFields(BindingFlags.Public |
                                                          BindingFlags.Static |
                                                          BindingFlags.FlattenHierarchy))
                             .Select(f => f.GetValue(null)?.ToString())
                             .Where(v => !string.IsNullOrEmpty(v) && v.Contains(displayNameAttribute[i].DisplayName))
                             .ToList();

            permissionInfo.Add(new Tuple<string, string, IEnumerable<string?>>(
                displayNameAttribute[i].DisplayName,
                descriptionAttributes[i].Description,
                permissions)
            );
        }

        return permissionInfo;
    }
}
