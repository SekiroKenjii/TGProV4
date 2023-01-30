namespace TGProV4.Application.Requests.Identity;

public class ResetPasswordRequest : ForgotPasswordRequest
{
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? Token { get; set; }
}
