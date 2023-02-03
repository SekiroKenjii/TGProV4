namespace TGProV4.Infrastructure.Services;

public class DataSeeder : IDataSeeder
{
    private readonly ILogger<DataSeeder> _logger;

    private readonly ApplicationDbContext _context;

    private readonly UserManager<AppUser> _userManager;

    private readonly RoleManager<AppRole> _roleManager;

    private string _adminId;

    public DataSeeder(
        ILogger<DataSeeder> logger,
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _adminId = string.Empty;
    }

    public void Run()
    {
        try {
            if (_context.Database.IsSqlServer())
            {
                _context.Database.Migrate();
            }

            AddAdministrator();
            AddBrands();

            var result = _context.SaveChanges();

            if (result > 0)
            {
                _logger.LogInformation("Done!");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while migrating or seeding the database.");

            throw;
        }
    }

    private void AddAdministrator()
    {
        Task.Run(async () => {
                 _logger.LogInformation("Initialize Administrator Role...");

                 var adminRole = new AppRole(ApplicationConstants.Roles.Administrator, "Full permissions role");
                 var roleFromDb = await _roleManager.FindByNameAsync(ApplicationConstants.Roles.Administrator);

                 if (roleFromDb is null)
                 {
                     await _roleManager.CreateAsync(adminRole);
                     roleFromDb = await _roleManager.FindByNameAsync(ApplicationConstants.Roles.Administrator);

                     if (roleFromDb is null)
                     {
                         _logger.LogError("An error has occurred: Failed to initialize administrator role");
                         return;
                     }

                     _logger.LogInformation("Done!");
                 }

                 _logger.LogInformation("Initialize Default Web Owner...");

                 var webOwner = new AppUser {
                     FirstName = "Thuong",
                     LastName = "Vo",
                     Email = "trungthuongvo109@gmail.com",
                     UserName = "trungthuongvo109",
                     AvatarUrl =
                         "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638705794/TGProV3/users/admin_avatar.jpg",
                     AvatarId = "TGProV3/users/admin_avatar",
                     EmailConfirmed = true,
                     PhoneNumber = "0375274267",
                     PhoneNumberConfirmed = true,
                     IsActive = true
                 };

                 var ownerFromDb = await _userManager.FindByEmailAsync(webOwner.Email);

                 if (ownerFromDb is null)
                 {
                     await _userManager.CreateAsync(webOwner, ApplicationConstants.Secrets.DefaultPassword);

                     ownerFromDb = await _userManager.FindByEmailAsync(webOwner.Email);

                     if (ownerFromDb is null)
                     {
                         _logger.LogError("An error has occurred: Failed to initialize default web owner");
                         return;
                     }

                     _adminId = ownerFromDb.Id;
                     ownerFromDb.CreatedBy = _adminId;
                     roleFromDb.CreatedBy = _adminId;

                     await _userManager.UpdateAsync(ownerFromDb);
                     await _roleManager.UpdateAsync(roleFromDb);

                     var result = await _userManager.AddToRoleAsync(webOwner, ApplicationConstants.Roles.Administrator);

                     if (result.Succeeded)
                     {
                         _logger.LogInformation("Done!");
                     }
                     else
                     {
                         foreach (var error in result.Errors)
                         {
                             _logger.LogError("An error has occurred: {description}", error.Description);
                         }
                     }
                 }

                 foreach (var permissionDetail in ConstantHelpers.GetApplicationPermissions())
                 {
                     await _context.AddPermissionClaim(roleFromDb, permissionDetail);
                 }
             })
            .GetAwaiter()
            .GetResult();
    }

    private void AddBrands()
    {
        if (_context.Brands is not null)
        {
            Task.Run(async () => {
                     if (!_context.Brands.Any())
                     {
                         _logger.LogInformation("Initialize Sample Brand Data...");

                         var brands = new List<Brand> {
                             new() {
                                 Name = "Dell",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/dell.png",
                                 LogoId = "TGProV3/brands/dell",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Lenovo",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/lenovo.png",
                                 LogoId = "TGProV3/brands/lenovo",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Asus",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/asus.png",
                                 LogoId = "TGProV3/brands/asus",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "MSI",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/msi.png",
                                 LogoId = "TGProV3/brands/msi",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "HP",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/hp.png",
                                 LogoId = "TGProV3/brands/hp",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Apple",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/apple.png",
                                 LogoId = "TGProV3/brands/apple",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Intel",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/intel.png",
                                 LogoId = "TGProV3/brands/intel",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "LG",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/lg.png",
                                 LogoId = "TGProV3/brands/lg",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Razer",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/razer.png",
                                 LogoId = "TGProV3/brands/razer",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Microsoft",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/microsoft.png",
                                 LogoId = "TGProV3/brands/microsoft",
                                 CreatedBy = _adminId
                             }
                         };

                         await _context.Brands.AddRangeAsync(brands);
                     }
                 })
                .GetAwaiter()
                .GetResult();
        }
        else
        {
            _logger.LogError("An error has occurred: {description}", "DbSet<Brand> is null");
        }
    }
}
