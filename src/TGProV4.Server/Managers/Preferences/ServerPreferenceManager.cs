namespace TGProV4.Server.Managers.Preferences;

public class ServerPreferenceManager : IServerPreferenceManager
{
    private readonly IStringLocalizer<ServerPreferenceManager> _localizer;
    private readonly IAsyncServerStorageService _asyncServerStorageService;

    public ServerPreferenceManager(
        IStringLocalizer<ServerPreferenceManager> localizer,
        IAsyncServerStorageService asyncServerStorageService)
    {
        _localizer = localizer;
        _asyncServerStorageService = asyncServerStorageService;
    }

    public async Task SetPreference(IPreference preference)
    {
        await _asyncServerStorageService.SetItemAsync(ApplicationConstants.Storage.Server.Preference,
            preference as ServerPreference);
    }

    public async Task<IPreference> GetPreference()
    {
        return await _asyncServerStorageService
                  .GetItemAsync<ServerPreference>(ApplicationConstants.Storage.Server.Preference) ??
               new ServerPreference();
    }

    public async Task<IResponse> ChangeLanguage(string code)
    {
        if (await GetPreference() is not ServerPreference preference)
        {
            return new Response {
                Succeeded = false,
                Message = _localizer["Failed to get server preferences"]
            };
        }

        preference.Language = code;

        await SetPreference(preference);

        return new Response {
            Succeeded = true,
            Message = _localizer["Server Language has been changed"]
        };
    }
}
