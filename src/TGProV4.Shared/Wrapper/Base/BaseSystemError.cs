namespace TGProV4.Shared.Wrapper.Base;

public class BaseSystemError : BaseError
{
    public string? FileName { get; set; }
    public string? Method { get; set; }
    public string? LineNumber { get; set; }
}
