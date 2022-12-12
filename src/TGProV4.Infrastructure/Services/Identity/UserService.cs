using Microsoft.AspNetCore.WebUtilities;

namespace TGProV4.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UserService(
        UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
        IMapper mapper, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<UserResponse>> GetAllUsers()
    {
        var users = await _userManager.Users.Select(x => new UserResponse
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            IsActive = x.IsActive,
            EmailConfirmed = x.EmailConfirmed,
            PhoneNumber = x.PhoneNumber,
            AvatarUrl = x.AvatarUrl
        })
            .ToListAsync();

        return users;
    }

    public async Task<int> CountUser()
    {
        var count = await _userManager.Users.CountAsync();

        return count;
    }

    public async Task<UserResponse?> GetUser(string userId)
    {
        var user = await _userManager.Users.Where(x => x.Id == userId)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                IsActive = x.IsActive,
                EmailConfirmed = x.EmailConfirmed,
                PhoneNumber = x.PhoneNumber,
                AvatarUrl = x.AvatarUrl
            })
            .FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException(StringHelpers.NotFound("User"));

        return user;
    }

    public async Task<bool> Register(RegisterRequest request)
    {
        var exists = await _userManager.FindByEmailAsync(request.Email);

        if (exists != null)
            throw new ValidationException(
                errorCode: "DuplicateDataValidator",
                message: StringHelpers.AlreadyTaken("Email"),
                propertyName: "Email");

        exists = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);

        if (exists != null)
            throw new ValidationException(
                errorCode: "DuplicateDataValidator",
                message: StringHelpers.AlreadyTaken("PhoneNumber"),
                propertyName: "PhoneNumber");

        var user = _mapper.Map<AppUser>(request);

        var createUser = await _userManager.CreateAsync(user, request.Password);

        if (!createUser.Succeeded) return false;

        var addBasicRole = await _userManager.AddToRoleAsync(user, ApplicationConstants.Roles.Basic);

        if (!addBasicRole.Succeeded) return false;

        if (!request.AutoConfirmEmail)
        {
            // @TODO - Send mail
        }

        return true;
    }

    public async Task<List<UserRoleResponse>> GetUserRoles(string userId)
    {
        var userRoles = new List<UserRoleResponse>();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new NotFoundException(StringHelpers.NotFound("User"));

        var roles = await _roleManager.Roles.ToListAsync();

        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleResponse
            {
                RoleName = role.Name,
                RoleDescription = role.Description,
                Selected = await _userManager.IsInRoleAsync(user, role.Name)
            });
        }

        return userRoles;
    }

    public async Task<bool> UpdateUserRoles(UpdateUserRolesRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
            throw new NotFoundException(StringHelpers.NotFound("User"));

        var userRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = request.Roles.Where(x => x.Selected).ToList();

        if (string.IsNullOrEmpty(_currentUserService.UserId))
            throw new UnauthorizedException(ApplicationConstants.Messages.Unauthorized);

        var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);

        if (currentUser == null)
            throw new NotFoundException(StringHelpers.NotFound("Current User"));

        if (!await _userManager.IsInRoleAsync(currentUser, ApplicationConstants.Roles.Administrator))
        {
            var addAdminRole = selectedRoles.Any(x => x.RoleName == ApplicationConstants.Roles.Administrator);
            var isAdministrator = userRoles.Any(x => x == ApplicationConstants.Roles.Administrator);

            if (addAdminRole && !isAdministrator || !addAdminRole && isAdministrator)
                throw new BadRequestException(ApplicationConstants.Messages.NotAllowToAddOrDeleteAdminRole);
        }

        var result = await _userManager.RemoveFromRolesAsync(user, userRoles);

        if (!result.Succeeded) return false;

        result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(x => x.RoleName));

        return result.Succeeded;
    }

    public async Task<bool> ConfirmEmail(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new NotFoundException(StringHelpers.NotFound("User"));

        var decode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ConfirmEmailAsync(user, decode);

        return result.Succeeded;
    }
}