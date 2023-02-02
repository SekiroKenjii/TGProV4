namespace TGProV4.Application.Interfaces.Services.Storage;

public interface ISyncServerStorageService
{
    void Clear();
    T? GetItem<T>(string key);
    string GetItemAsString(string key);
    string Key(int index);
    bool ContainKey(string key);
    int Length();
    void RemoveItem(string key);
    void SetItem<T>(string key, T data);
    void SetItemAsString(string key, string data);

    event EventHandler<ChangingEventArgs> Changing;
    event EventHandler<ChangedEventArgs> Changed;
}
