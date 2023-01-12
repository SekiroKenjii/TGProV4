namespace TGProV4.Shared.Wrapper.Base;

public class BaseValidationError : BaseError
{
    public string? Code { get; set; }
    public string? RelatedProperties { get; set; }
    public string? Message { get; set; }
}
