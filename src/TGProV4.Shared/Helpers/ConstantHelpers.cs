using System.Reflection;
using TGProV4.Shared.Constants.Application;

namespace TGProV4.Shared.Helpers;

public static class ConstantHelpers
{
    public static List<string> GetApplicationPermissions()
    {
        return (from field in typeof(ApplicationPermissions).GetNestedTypes()
                                                            .SelectMany(c
                                                                 => c.GetFields(BindingFlags.Public |
                                                                     BindingFlags.Static |
                                                                     BindingFlags.FlattenHierarchy))
                select field.GetValue(null)
                into propertyValue
                where propertyValue is not null
                select propertyValue.ToString()).ToList();
    }
}
