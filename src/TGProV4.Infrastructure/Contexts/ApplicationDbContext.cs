namespace TGProV4.Infrastructure.Contexts;

public class ApplicationDbContext : AuditableDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
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
        {
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
        }

        return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var properties = builder.Model.GetEntityTypes()
            .SelectMany(type => type.GetProperties())
            .ToList();

        foreach (var property in properties.Where(prop =>
                     prop.ClrType == typeof(decimal) || prop.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        foreach (var property in properties.Where(prop =>
                     prop.DeclaringEntityType.Name is "CreatedBy" or "LastModifiedBy"))
        {
            property.SetColumnType("nvarchar(128)");
        }

        base.OnModelCreating(builder);

        // AspNetCore Identity
        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users", "Identity");
            
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
        });

        builder.Entity<AppRole>(entity =>
        {
            entity.ToTable("Roles", "Identity");
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles", "Identity");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims", "Identity");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins", "Identity");
        });

        builder.Entity<AppRoleClaim>(entity =>
        {
            entity.ToTable("RoleClaims", "Identity");

            entity.HasOne(x => x.Role)
                .WithMany(y => y.RoleClaims)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AppUserToken>(entity =>
        {
            entity.ToTable("UserTokens", "Identity");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.HasOne(x => x.User)
                .WithMany(y => y.UserTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(x => x.Expires).IsRequired().HasDefaultValue(DateTimeOffset.Now.AddDays(7));
            entity.Property(x => x.Revoked).IsRequired(false);
        });

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
