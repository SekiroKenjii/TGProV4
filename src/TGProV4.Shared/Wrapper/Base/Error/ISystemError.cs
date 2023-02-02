namespace TGProV4.Shared.Wrapper.Base.Error;

public interface ISystemError : IError
{
    public string? FileName { get; set; }
    public string? Method { get; set; }
    public string? LineNumber { get; set; }
}
