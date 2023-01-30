namespace TGProV4.Infrastructure.Configurations.Identity;

public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("Roles", ApplicationConstants.TableSchemas.Identity);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.NormalizedName).IsUnique();
        
        builder.Property(x => x.Name).IsRequired().HasColumnType("nvarchar(20)");
        builder.Property(x => x.Description).IsRequired(false).HasColumnType("nvarchar(255)");
        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
        builder.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
        builder.Property(x => x.LastModifiedAt).IsRequired(false);
    }
}
