using FluentValidation;

namespace TGProV4.Application.Validators.Requests.Identity;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().Length(100);
        RuleFor(x => x.LastName).NotEmpty().Length(100);
        RuleFor(x => x.PhoneNumber).NotEmpty().Length(16);
        RuleFor(x => x.Email).NotEmpty().Length(255);
    }
}
