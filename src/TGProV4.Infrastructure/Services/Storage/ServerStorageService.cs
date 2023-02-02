using JsonException=System.Text.Json.JsonException;

namespace TGProV4.Infrastructure.Services.Storage;

public class ServerStorageService : IAsyncServerStorageService, ISyncServerStorageService
{
    private readonly IStorageProvider _storageProvider;

    private readonly IJsonSerializer _serializer;

    public ServerStorageService(IStorageProvider storageProvider, IJsonSerializer serializer)
    {
        _storageProvider = storageProvider;
        _serializer = serializer;
    }

    public T? GetItem<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var serializedData = _storageProvider.GetItem(key);

        if (string.IsNullOrWhiteSpace(serializedData))
        {
            return default;
        }

        try
        {
            return _serializer.Deserialize<T>(serializedData);
        }
        catch (JsonException e) when (e.Path == "$" && typeof(T) == typeof(string))
        {
            return (T) (object) serializedData;
        }
    }

    public string GetItemAsString(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        return _storageProvider.GetItem(key);
    }

    public string Key(int index) { return _storageProvider.Key(index); }

    public bool ContainKey(string key) { return _storageProvider.ContainKey(key); }

    public int Length() { return _storageProvider.Length(); }

    public void RemoveItem(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        _storageProvider.RemoveItem(key);
    }

    public void SetItem<T>(string key, T data)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var e = RaiseOnChangingSync(key, data);

        if (e.Cancel)
        {
            return;
        }

        var serializedData = _serializer.Serialize(data);

        _storageProvider.SetItem(key, serializedData);

        RaiseOnChanged(key, e.OldValue, data);
    }

    public void SetItemAsString(string key, string data)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var e = RaiseOnChangingSync(key, data);

        if (e.Cancel)
        {
            return;
        }

        _storageProvider.SetItem(key, data);

        RaiseOnChanged(key, e.OldValue, data);
    }

    public void Clear() { _storageProvider.Clear(); }

    public async ValueTask<T?> GetItemAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var serializedData = await _storageProvider.GetItemAsync(key).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(serializedData))
        {
            return default;
        }

        try
        {
            return _serializer.Deserialize<T>(serializedData);
        }
        catch (JsonException e) when (e.Path == "$" && typeof(T) == typeof(string))
        {
            return (T)(object)serializedData;
        }
    }

    public ValueTask<string> GetItemAsStringAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        return _storageProvider.GetItemAsync(key);
    }

    public ValueTask<string> KeyAsync(int index) { return _storageProvider.KeyAsync(index); }

    public ValueTask<bool> ContainKeyAsync(string key) { return _storageProvider.ContainKeyAsync(key); }

    public ValueTask<int> LengthAsync() { return _storageProvider.LengthAsync(); }

    public ValueTask RemoveItemAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        return _storageProvider.RemoveItemAsync(key);
    }

    public async ValueTask SetItemAsync<T>(string key, T data)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var e = await RaiseOnChangingAsync(key, data).ConfigureAwait(false);

        if (e.Cancel)
        {
            return;
        }

        var serializedData = _serializer.Serialize(data);

        await _storageProvider.SetItemAsync(key, serializedData).ConfigureAwait(false);

        RaiseOnChanged(key, e.OldValue, data);
    }

    public async ValueTask SetItemAsStringAsync(string key, string data)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var e = await RaiseOnChangingAsync(key, data).ConfigureAwait(false);

        if (e.Cancel)
        {
            return;
        }

        await _storageProvider.SetItemAsync(key, data).ConfigureAwait(false);

        RaiseOnChanged(key, e.OldValue, data);
    }

    public ValueTask ClearAsync() { return _storageProvider.ClearAsync(); }

    public event EventHandler<ChangingEventArgs>? Changing;

    public event EventHandler<ChangedEventArgs>? Changed;

    private async Task<ChangingEventArgs> RaiseOnChangingAsync(string key, object? data)
    {
        var e = new ChangingEventArgs {
            Key = key,
            OldValue = await GetItemInternalAsync<object>(key).ConfigureAwait(false),
            NewValue = data
        };

        Changing?.Invoke(this, e);

        return e;
    }

    private ChangingEventArgs RaiseOnChangingSync(string key, object? data)
    {
        var e = new ChangingEventArgs {
            Key = key,
            OldValue = GetItemInternal(key),
            NewValue = data
        };

        Changing?.Invoke(this, e);

        return e;
    }

    private async Task<T?> GetItemInternalAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var serializedData = await _storageProvider.GetItemAsync(key).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(serializedData))
        {
            return default;
        }

        try
        {
            return _serializer.Deserialize<T>(serializedData);
        }
        catch (JsonException)
        {
            return (T) (object) serializedData;
        }
    }

    private object? GetItemInternal(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var serializedData = _storageProvider.GetItem(key);

        if (string.IsNullOrWhiteSpace(serializedData))
        {
            return default;
        }

        try
        {
            return _serializer.Deserialize<object>(serializedData);
        }
        catch (JsonException)
        {
            return serializedData;
        }
    }

    private void RaiseOnChanged(string key, object? oldValue, object? data)
    {
        var e = new ChangedEventArgs {
            Key = key,
            OldValue = oldValue,
            NewValue = data
        };

        Changed?.Invoke(this, e);
    }
}
