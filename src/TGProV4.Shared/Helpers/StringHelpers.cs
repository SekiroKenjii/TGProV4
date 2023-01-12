namespace TGProV4.Shared.Helpers;

public static class StringHelpers
{
    public static class Message
    {
        public static string AlreadyTaken(string prop) { return $"{prop} is already taken."; }

        public static string NotFound(string entity) { return $"{entity} Not Found!"; }

        public static string MissedConfig(string entity) { return $"Missed config of {entity}"; }
    }
}
