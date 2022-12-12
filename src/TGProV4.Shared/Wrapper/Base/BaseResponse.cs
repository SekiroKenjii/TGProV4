namespace TGProV4.Shared.Wrapper.Base;

public class BaseResponse
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<BaseError> Errors { get; set; } = new();
}
