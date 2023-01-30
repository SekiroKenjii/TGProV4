using Microsoft.Extensions.Logging;
using TGProV4.Infrastructure.Extensions;

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
        AddAdministrator();
        AddBrands();

        var result = _context.SaveChanges();

        if (result > 0)
        {
            _logger.LogInformation("Done!");
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
                     roleFromDb.CreatedBy = _adminId;

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
                                 Description =
                                     "Dell là thương hiệu có quá trình phát triển lâu dài và bền bỉ trong ngành công nghiệp máy tính. Dell cung cấp nhiều dòng laptop chất lượng, cao cấp như XPS, Precision, Latitude và nổi bật với G-Series Gaming và Alienware hàng đầu dành cho game thủ.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Lenovo",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/lenovo.png",
                                 LogoId = "TGProV3/brands/lenovo",
                                 Description =
                                     "Lenovo đặc biệt thành công với dòng laptop doanh nhân cao cấp ThinkPad lâu đời và mở rộng các dòng sản phẩm mới mang tính sáng tạo IdeaPad, Legion dành cho gaming và ThinkBook nhắm tới đối tượng học sinh, sinh viên.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Asus",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/asus.png",
                                 LogoId = "TGProV3/brands/asus",
                                 Description =
                                     "ASUS là thương hiệu laptop số 1 thế giới ở thời điểm hiện tại. Dòng sản phẩm đa dạng, phục vụ nhiều nhu cầu và luôn mang một chất riêng giúp ASUS ghi điểm với người dùng toàn cầu.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "MSI",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/msi.png",
                                 LogoId = "TGProV3/brands/msi",
                                 Description =
                                     "MSI (tên đầy đủ: Micro-Star International Co., Ltd) - Một trong những hãng sản xuất Laptop gaming lớn nhất Đài Loan. MSI là doanh nghiệp công nghệ toàn cầu và uy tín trên thế giới.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "HP",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/hp.png",
                                 LogoId = "TGProV3/brands/hp",
                                 Description =
                                     "HP sản xuất máy tính xách tay phục vụ mọi nhu cầu bao gồm cả cho gia đình và văn phòng tại nhà, công ty và doanh nghiệp. Đồng thời HP cung cấp đa dạng nhu cầu với dải sản phẩm rất rộng bao gồm EliteBook, ZBook, Envy, Spectre, Pavilion, EliteDesk.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Apple",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/apple.png",
                                 LogoId = "TGProV3/brands/apple",
                                 Description =
                                     "Apple thành công khi đưa ra một hệ sinh thái riêng sử dụng nền tảng hệ điều hành do chính hãng phát triển. Các dòng laptop, PC của hãng đều có độ ổn định, hiệu năng và thời lượng pin vượt trội so với phần còn lại.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Intel",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/intel.png",
                                 LogoId = "TGProV3/brands/intel",
                                 Description =
                                     "Tập đoàn Intel (Integrated Electronics) thành lập năm 1968 tại Santa Clara, California, Hoa Kỳ, là nhà sản xuất các sản phẩm như chip vi xử lý cho máy tính, bo mạch chủ, ổ nhớ flash, card mạng và các thiết bị máy tính khác.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "LG",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/lg.png",
                                 LogoId = "TGProV3/brands/lg",
                                 Description =
                                     "Máy tính xách tay LG thế hệ mới mang vẻ đẹp cao cấp cùng nhiều tính năng đáng giá, mang thiết kế mỏng nhẹ, tinh tế cùng cấu hình cao. Laptop LG chính hãng phù hợp với nhu cầu học tập, giải trí và tạo ra những trải nghiệm thú vị cho người dùng.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Razer",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/razer.png",
                                 LogoId = "TGProV3/brands/razer",
                                 Description =
                                     "Razer là hãng sản xuất thiết bị Gaming nổi tiếng với sự cao cấp và thiết kế hàng đầu. Đặc biệt, Laptop Gaming Razer Blade luôn mang lại những sự tinh tế và hiệu suất khác biệt.",
                                 CreatedBy = _adminId
                             },
                             new() {
                                 Name = "Microsoft",
                                 LogoUrl =
                                     "https://res.cloudinary.com/dglgzh0aj/image/upload/v1638721714/TGProV3/brands/microsoft.png",
                                 LogoId = "TGProV3/brands/microsoft",
                                 Description =
                                     "Microsoft vốn là công ty phần mềm đứng đầu thế giới với hệ điều hành Windows phổ biến. Trong vài năm trở lại đây, Microsoft đang tham gia vào thị trường máy tính với dòng sản phẩm Surface và ngay lập tức được người dùng đón nhận.",
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
