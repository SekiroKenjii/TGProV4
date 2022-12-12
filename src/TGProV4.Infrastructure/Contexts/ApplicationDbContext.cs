﻿namespace TGProV4.Infrastructure.Contexts;

public class ApplicationDbContext : AuditableDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    // public DbSet<Entity>? Entities { get; set; }

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
                    entry.Entity.CreatedOn = DateTimeOffset.Now;
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = DateTimeOffset.Now;
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
        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(type => type.GetProperties())
                     .Where(property => property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(type => type.GetProperties())
                     .Where(property => property.Name is "LastModifiedBy" or "CreatedBy"))
        {
            property.SetColumnType("nvarchar(128)");
        }

        base.OnModelCreating(builder);

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

            entity.HasOne(x => x.User)
                .WithMany(y => y.UserTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
