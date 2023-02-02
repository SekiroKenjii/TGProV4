namespace TGProV4.Application.Serialization.Serializers;

public class SystemTextJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SystemTextJsonSerializer(IOptions<SystemTextJsonOptions> options)
        => _jsonSerializerOptions = options.Value.JsonSerializerOptions;

    public string Serialize<T>(T obj) { return JsonSerializer.Serialize(obj, _jsonSerializerOptions); }

    public T? Deserialize<T>(string data) { return JsonSerializer.Deserialize<T>(data, _jsonSerializerOptions); }
}
