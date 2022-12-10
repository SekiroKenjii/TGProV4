namespace TGProV4.Application.Exceptions;

public class ValidationException : Exception
{
    private readonly List<BaseError> _errors;

    private ValidationException() : base(ApplicationConstants.Messages.ValidationError)
    {
        _errors = new List<BaseError>();
    }
    
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        foreach (var failure in failures)
        {
            _errors.Add(new BaseError
            {
                Code = failure.ErrorCode,
                Message = failure.ErrorMessage,
                RelatedProperties = failure.PropertyName
            });
        }
    }
    
    public ValidationException(string errorCode, string message, string propertyName)
        : this()
    {
        _errors.Add(new BaseError
        {
            Code = errorCode,
            Message = message,
            RelatedProperties = propertyName
        });
    }

    public List<BaseError> GetErrors() => _errors;
}