namespace TGProV4.Infrastructure.Configurations.Identity;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users", ApplicationConstants.TableSchemas.Identity);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasIndex(x => x.PhoneNumber).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.AvatarId).IsUnique();
        builder.HasIndex(x => x.IsActive).IsUnique();
        builder.HasIndex(x => x.IsDeleted).IsUnique();
        
        builder.Property(x => x.FirstName).IsRequired().HasColumnType("nvarchar(100)");
        builder.Property(x => x.LastName).IsRequired().HasColumnType("nvarchar(100)");
        builder.Property(x => x.PhoneNumber).IsRequired().HasColumnType("varchar(16)");
        builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.AvatarUrl).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.AvatarId).IsRequired().HasColumnType("varchar(250)");
        builder.Property(x => x.Gender).IsRequired().HasColumnType("TINYINT").HasDefaultValue(Gender.Undefined);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
        builder.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
        builder.Property(x => x.LastModifiedAt).IsRequired(false);
        builder.Property(x => x.DeletedOn).IsRequired(false);
    }
}
