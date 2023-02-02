namespace TGProV4.Shared.Wrapper.Error;

public class SystemError : ISystemError
{
    public string? FileName { get; set; }
    public string? Method { get; set; }
    public string? LineNumber { get; set; }
    public string? StackTrace { get; set; }
}
