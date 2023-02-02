namespace TGProV4.Shared.Wrapper;

public class Response<T> : Response
{
    public T? Data { get; set; }
}

public class Response : IResponse
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; } = string.Empty;
    public ICollection<IError> Errors { get; set; } = new List<IError>();
}
