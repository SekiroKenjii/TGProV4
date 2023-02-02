namespace TGProV4.Shared.Wrapper.Base;

public interface IResponse
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public ICollection<IError> Errors { get; set; }
}
