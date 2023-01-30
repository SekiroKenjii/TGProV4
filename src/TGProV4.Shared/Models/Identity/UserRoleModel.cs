namespace TGProV4.Shared.Models.Identity;

public class UserRoleModel
{
    public string RoleName { get; init; } = string.Empty;
    public string? RoleDescription { get; set; }
    public bool Selected { get; init; }
}
