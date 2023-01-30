namespace TGProV4.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IMailService _mailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHeaderDictionary? _headerDictionary;

    public UserService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IMapper mapper,
        IMailService mailService,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _mailService = mailService;
        _currentUserService = currentUserService;
        _headerDictionary = httpContextAccessor.HttpContext?.Request.Headers;
    }

    public async Task<List<UserResponse>> GetAllUsers()
    {
        var users = await _userManager.Users.Select(x => new UserResponse {
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
                                     .Select(x => new UserResponse {
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

        if (user is null)
        {
            throw new NotFoundException(StringHelpers.Message.NotFound("User"));
        }

        return user;
    }

    public async Task<bool> Register(RegisterRequest request)
    {
        var exists = await _userManager.FindByEmailAsync(request.Email!);

        if (exists != null)
        {
            throw new ValidationException("DuplicateDataValidator", StringHelpers.Message.AlreadyTaken("Email"),
                "Email");
        }

        exists = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);

        if (exists != null)
        {
            throw new ValidationException("DuplicateDataValidator", StringHelpers.Message.AlreadyTaken("PhoneNumber"),
                "PhoneNumber");
        }

        var user = _mapper.Map<AppUser>(request);

        var createUser = await _userManager.CreateAsync(user, request.Password!);

        if (!createUser.Succeeded)
        {
            return false;
        }

        var addBasicRole = await _userManager.AddToRoleAsync(user, ApplicationConstants.Roles.Basic);

        if (!addBasicRole.Succeeded)
        {
            return false;
        }

        // ReSharper disable once InvertIf
        if (!request.AutoConfirmEmail)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

            var origin = _headerDictionary?["origins"];

            if (!origin.HasValue)
            {
                throw new BadRequestException(ApplicationConstants.Messages.HeaderError);
            }

            var endpoint = new Uri(string.Concat($"{origin.Value}/", ApplicationConstants.Routes.ConfirmEmail));
            var confirmationUrl = QueryHelpers.AddQueryString(endpoint.ToString(), "UserId", user.Id);
            confirmationUrl = QueryHelpers.AddQueryString(confirmationUrl, "EmailConfirmationToken", emailConfirmationToken);
            var mailRequest = new MailRequest
            {
                Subject = "Confirm Registration For TGPro Account",
                Body = StringHelpers.Mail.ConfirmEmailBody(confirmationUrl, user.FirstName, user.LastName),
                To = user.Email
            };

            BackgroundJob.Enqueue(() => _mailService.Send(mailRequest));
        }

        return true;
    }

    public async Task<List<UserRoleResponse>> GetUserRoles(string userId)
    {
        var userRoles = new List<UserRoleResponse>();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException(StringHelpers.Message.NotFound("User"));
        }

        var roles = await _roleManager.Roles.ToListAsync();

        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleResponse {
                RoleName = role.Name!,
                RoleDescription = role.Description,
                Selected = await _userManager.IsInRoleAsync(user, role.Name!)
            });
        }

        return userRoles;
    }

    public async Task<bool> UpdateUserRoles(UpdateUserRolesRequest request)
    {
        if (string.IsNullOrEmpty(request.UserId))
        {
            throw new ValidationException("PropertyIsNullOrEmpty", ApplicationConstants.Messages.ValidationError,
                "UserId");
        }

        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            throw new NotFoundException(StringHelpers.Message.NotFound("User"));
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = request.Roles.Where(x => x.Selected).ToList();

        if (string.IsNullOrEmpty(_currentUserService.UserId))
        {
            throw new UnauthorizedException(ApplicationConstants.Messages.Unauthorized);
        }

        var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);

        if (currentUser is null)
        {
            throw new NotFoundException(StringHelpers.Message.NotFound("Current User"));
        }

        if (!await _userManager.IsInRoleAsync(currentUser, ApplicationConstants.Roles.Administrator))
        {
            var addAdminRole = selectedRoles.Any(x => x.RoleName is ApplicationConstants.Roles.Administrator);
            var isAdministrator = userRoles.Any(x => x is ApplicationConstants.Roles.Administrator);

            if (addAdminRole && !isAdministrator || !addAdminRole && isAdministrator)
            {
                throw new BadRequestException(ApplicationConstants.Messages.NotAllowToAddOrDeleteAdminRole);
            }
        }

        var result = await _userManager.RemoveFromRolesAsync(user, userRoles);

        if (!result.Succeeded)
        {
            return false;
        }

        result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(x => x.RoleName));

        return result.Succeeded;
    }

    public async Task<bool> ConfirmEmail(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException(StringHelpers.Message.NotFound("User"));
        }

        var decode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ConfirmEmailAsync(user, decode);

        return result.Succeeded;
    }
    public async Task<string> ForgotPassword(ForgotPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            throw new ValidationException("PropertyIsNullOrEmpty", ApplicationConstants.Messages.ValidationError,
                "Email");
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        user = user.ValidateUser();

        var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        resetPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));

        var origin = _headerDictionary?["origins"];

        if (!origin.HasValue)
        {
            throw new BadRequestException(ApplicationConstants.Messages.HeaderError);
        }

        var endpoint = new Uri(string.Concat($"{origin.Value}/", ApplicationConstants.Routes.ResetPassword));
        var resetPasswordUrl =
            QueryHelpers.AddQueryString(endpoint.ToString(), "ResetPasswordToken", resetPasswordToken);
        var mailRequest = new MailRequest {
            Subject = "Reset Password Request For TGPro Account",
            Body = StringHelpers.Mail.ChangePasswordBody(HtmlEncoder.Default.Encode(resetPasswordUrl), user.FirstName,
                user.LastName),
            To = request.Email
        };

        BackgroundJob.Enqueue(() => _mailService.Send(mailRequest));

        return ApplicationConstants.Messages.MailSent;
    }
    public async Task<bool> ResetPassword(ResetPasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            throw new ValidationException("PropertyIsNullOrEmpty", ApplicationConstants.Messages.ValidationError,
                "Email");
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        user = user.ValidateUser();

        if (string.IsNullOrEmpty(request.Token))
        {
            throw new ValidationException("PropertyIsNullOrEmpty", ApplicationConstants.Messages.ValidationError,
                "Token");
        }

        if (string.IsNullOrEmpty(request.Password))
        {
            throw new ValidationException("PropertyIsNullOrEmpty", ApplicationConstants.Messages.ValidationError,
                "Password");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

        return result.Succeeded;
    }
}
