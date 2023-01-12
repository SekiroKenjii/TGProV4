namespace TGProV4.Infrastructure.Contexts;

public class ApplicationDbContext : AuditableDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
        : base(options) => _currentUserService = currentUserService;

    public DbSet<Brand>? Brands { get; [UsedImplicitly] set; }
    public DbSet<Category>? Categories { get; [UsedImplicitly] set; }
    public DbSet<Color>? Colors { get; [UsedImplicitly] set; }
    public DbSet<Product>? Products { get; [UsedImplicitly] set; }
    public DbSet<ProductColor>? ProductColors { get; [UsedImplicitly] set; }
    public DbSet<ProductCondition>? ProductConditions { get; [UsedImplicitly] set; }
    public DbSet<ProductDetail>? ProductDetails { get; [UsedImplicitly] set; }
    public DbSet<ProductImage>? ProductImages { get; [UsedImplicitly] set; }
    public DbSet<ProductType>? ProductTypes { get; [UsedImplicitly] set; }
    public DbSet<SubBrand>? SubBrands { get; [UsedImplicitly] set; }

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
        builder.Entity<AppUser>(entity => {
            entity.ToTable("Users", "Identity");

            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
            entity.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
            entity.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
            entity.Property(x => x.LastModifiedAt).IsRequired(false);
        });

        builder.Entity<AppRole>(entity => {
            entity.ToTable("Roles", "Identity");

            entity.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
            entity.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
            entity.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
            entity.Property(x => x.LastModifiedAt).IsRequired(false);
        });

        builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles", "Identity"); });

        builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims", "Identity"); });

        builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins", "Identity"); });

        builder.Entity<AppRoleClaim>(entity => {
            entity.ToTable("RoleClaims", "Identity");

            entity.HasOne(x => x.Role)
                  .WithMany(y => y.RoleClaims)
                  .HasForeignKey(x => x.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
            entity.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
            entity.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
            entity.Property(x => x.LastModifiedAt).IsRequired(false);
        });

        builder.Entity<AppUserToken>(entity => {
            entity.ToTable("UserTokens", "Identity");

            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.HasOne(x => x.User)
                  .WithMany(y => y.UserTokens)
                  .HasForeignKey(x => x.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(x => x.Expires)
                  .IsRequired()
                  .HasDefaultValue(DateTimeOffset.Now.AddDays(7));
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
