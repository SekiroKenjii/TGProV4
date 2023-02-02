namespace TGProV4.Server.Localization;

public class ServerLocalizer<T> where T : class
{
    // ReSharper disable once MemberCanBePrivate.Global
    public IStringLocalizer<T> Localizer { get; }

    public ServerLocalizer(IStringLocalizer<T> localizer) => Localizer = localizer;
}
