namespace TGProV4.Infrastructure.Contexts;

public class ApplicationDbContext : AuditableDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
        : base(options) => _currentUserService = currentUserService;

    public override DbSet<AppRoleClaim> RoleClaims {
        get => Set<AppRoleClaim>();
    }

    public override DbSet<AppRole> Roles {
        get => Set<AppRole>();
    }

    public override DbSet<AppUser> Users {
        get => Set<AppUser>();
    }

    public override DbSet<AppUserToken> UserTokens {
        get => Set<AppUserToken>();
    }

    public DbSet<Brand>? Brands { get; set; }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Color>? Colors { get; set; }
    public DbSet<Product>? Products { get; set; }
    public DbSet<ProductColor>? ProductColors { get; set; }
    public DbSet<ProductCondition>? ProductConditions { get; set; }
    public DbSet<ProductDetail>? ProductDetails { get; set; }
    public DbSet<ProductImage>? ProductImages { get; set; }
    public DbSet<ProductType>? ProductTypes { get; set; }
    public DbSet<SubBrand>? SubBrands { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        if (_currentUserService.UserId == null)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTimeOffset.Now;
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTimeOffset.Now;
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    break;

                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                default:
                    break;
            }

        return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // AspNetCore Identity
        builder.Entity<IdentityUserRole<string>>(entity => {
            entity.ToTable("UserRoles", "Identity");
        });
        builder.Entity<IdentityUserClaim<string>>(entity => {
            entity.ToTable("UserClaims", "Identity");
        });
        builder.Entity<IdentityUserLogin<string>>(entity => {
            entity.ToTable("UserLogins", "Identity");
        });
        builder.ApplyConfiguration(new AppRoleClaimConfiguration());
        builder.ApplyConfiguration(new AppRoleConfiguration());
        builder.ApplyConfiguration(new AppUserConfiguration());
        builder.ApplyConfiguration(new AppUserTokenConfiguration());

        // Model Configurations
        builder.ApplyConfiguration(new BrandConfiguration());
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new ColorConfiguration());
        builder.ApplyConfiguration(new ProductColorConfiguration());
        builder.ApplyConfiguration(new ProductConditionConfiguration());
        builder.ApplyConfiguration(new ProductConfiguration());
        builder.ApplyConfiguration(new ProductDetailConfiguration());
        builder.ApplyConfiguration(new ProductImageConfiguration());
        builder.ApplyConfiguration(new ProductTypeConfiguration());
        builder.ApplyConfiguration(new SubBrandConfiguration());
    }
}
