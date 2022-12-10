namespace TGProV4.Shared.Helpers;

public static class StringHelpers
{
    public static string AlreadyTaken(string prop) => $"{prop} is already taken.";
    public static string NotFound(string entity) => $"{entity} Not Found!"; 
    public static string MissedConfig(string entity) => $"Missed config of {entity}";
}