namespace TGProV4.Infrastructure.Configurations;

public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
{
    public void Configure(EntityTypeBuilder<ProductType> builder)
    {
        builder.ToTable("ProductTypes", "Production");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).IsRequired().HasColumnType("nvarchar(50)");
        builder.Property(x => x.Description).IsRequired(false).HasColumnType("nvarchar(500)");
        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
        builder.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
        builder.Property(x => x.LastModifiedAt).IsRequired(false);
    }
}
