namespace TGProV4.Infrastructure.Services.Storage.Provider;

public class ServerStorageProvider : IStorageProvider
{
    private readonly Dictionary<string, string> _storage = new();

    public void Clear() { _storage.Clear(); }

    public ValueTask ClearAsync()
    {
        Clear();

        return ValueTask.CompletedTask;
    }

    public bool ContainKey(string key) { return _storage.ContainsKey(key); }

    public ValueTask<bool> ContainKeyAsync(string key) { return ValueTask.FromResult(ContainKey(key)); }

    public string GetItem(string key) { return !_storage.ContainsKey(key) ? string.Empty : _storage[key]; }

    public ValueTask<string> GetItemAsync(string key)
    {
        return ValueTask.FromResult(GetItem(key));
    }

    public string Key(int index) { return _storage.ElementAt(index).Key; }

    public ValueTask<string> KeyAsync(int index) { return ValueTask.FromResult(Key(index)); }

    public int Length() { return _storage.Count; }

    public ValueTask<int> LengthAsync() { return ValueTask.FromResult(Length()); }

    public void RemoveItem(string key)
    {
        if (!_storage.ContainsKey(key))
        {
            return;
        }

        _storage.Remove(key);
    }

    public ValueTask RemoveItemAsync(string key)
    {
        RemoveItem(key);

        return ValueTask.CompletedTask;
    }

    public void SetItem(string key, string data)
    {
        if (_storage.ContainsKey(key))
        {
            _storage[key] = data;
            return;
        }

        _storage.Add(key, data);
    }

    public ValueTask SetItemAsync(string key, string data)
    {
        SetItem(key, data);

        return ValueTask.CompletedTask;
    }
}
