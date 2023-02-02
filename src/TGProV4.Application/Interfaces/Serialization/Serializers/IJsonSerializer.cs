namespace TGProV4.Application.Interfaces.Serialization.Serializers;

public interface IJsonSerializer
{
    string Serialize<T>(T obj);
    T? Deserialize<T>(string data);
}
