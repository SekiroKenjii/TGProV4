namespace TGProV4.Server.Settings;

public class ServerPreference : IPreference
{
    public string? Language { get; set; } = LocalizationConstants.Languages.FirstOrDefault()?.Code ?? "en-US";
}
