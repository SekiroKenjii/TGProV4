namespace TGProV4.Infrastructure.Configurations;

public class SubBrandConfiguration : IEntityTypeConfiguration<SubBrand>
{
    public void Configure(EntityTypeBuilder<SubBrand> builder)
    {
        builder.ToTable("SubBrands", "Production");

        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Brand)
            .WithMany(x => x.SubBrands)
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Category)
            .WithMany(x => x.SubBrands)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(x => x.Name).IsRequired().HasColumnType("nvarchar(50)");
        builder.Property(x => x.Description).IsRequired(false).HasColumnType("nvarchar(500)");
        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
        builder.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
        builder.Property(x => x.LastModifiedAt).IsRequired(false);
    }
}
