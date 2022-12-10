namespace TGProV4.Application.Requests.Identity;

public class TokenRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}