namespace TGProV4.Shared.Wrapper.Error;

public class ValidationError : IValidationError
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public string? PropertyName { get; set; }
    public string? StackTrace { get; set; }
}
