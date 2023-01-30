namespace TGProV4.Application.Interfaces.Services.Identity;

public interface IUserService
{
    Task<List<UserResponse>> GetAllUsers();
    Task<int> CountUser();
    Task<UserResponse?> GetUser(string userId);
    Task<bool> Register(RegisterRequest request);
    Task<List<UserRoleResponse>> GetUserRoles(string userId);
    Task<bool> UpdateUserRoles(UpdateUserRolesRequest request);
    Task<bool> ConfirmEmail(string userId, string code);
    Task<string> ForgotPassword(ForgotPasswordRequest request);
    Task<bool> ResetPassword(ResetPasswordRequest request);
}
