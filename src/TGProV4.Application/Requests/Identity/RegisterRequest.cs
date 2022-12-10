namespace TGProV4.Application.Requests.Identity;

public class RegisterRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? PhoneNumber { get; set; }
    public bool ActivateUser { get; set; } = false;
    public bool AutoConfirmEmail { get; set; } = false;
}