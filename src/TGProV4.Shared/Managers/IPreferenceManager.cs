namespace TGProV4.Shared.Managers;

public interface IPreferenceManager
{
    Task SetPreference(IPreference preference);

    Task<IPreference> GetPreference();

    Task<IResponse> ChangeLanguage(string code);
}
