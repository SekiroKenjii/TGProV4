using TGProV4.Shared.Models.Identity;

namespace TGProV4.Application.Requests.Identity;

public class UpdateUserRolesRequest
{
    public string? UserId { get; set; }
    public List<UserRoleModel> Roles { get; set; } = new();
}
