namespace TGProV4.Application.Requests.Identity;

public class UpdateUserProfileRequest : ImageUploadRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
