namespace TGProV4.Application.Exceptions;

public sealed class ValidationException : Exception
{
    private readonly ICollection<IError> _errors;

    private ValidationException() : base(ApplicationConstants.Messages.ValidationError)
        => _errors = new List<IError>();

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        foreach (var failure in failures)
        {
            _errors.Add(new ValidationError {
                Code = failure.ErrorCode,
                Message = failure.ErrorMessage,
                PropertyName = failure.PropertyName,
                StackTrace = StackTrace
            });
        }
    }

    public ValidationException(string errorCode, string message, string propertyName) : this()
        => _errors.Add(
            new ValidationError {
                Code = errorCode,
                Message = message,
                PropertyName = propertyName,
                StackTrace = StackTrace
            });

    public ICollection<IError> GetErrors() { return _errors; }
}
