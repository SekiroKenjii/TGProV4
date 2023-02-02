namespace TGProV4.Shared.Wrapper.Base.Error;

public interface IValidationError : IError
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public string? PropertyName { get; set; }
}
