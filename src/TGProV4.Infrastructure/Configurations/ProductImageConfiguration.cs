namespace TGProV4.Infrastructure.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages", ApplicationConstants.TableSchemas.Production);

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Product)
               .WithMany(x => x.Images)
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.ImageUrl).IsRequired().HasColumnType("nvarchar(255)");
        builder.Property(x => x.ImageId).IsRequired().HasColumnType("nvarchar(200)");
        builder.Property(x => x.Caption).IsRequired().HasColumnType("nvarchar(100)");
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
        builder.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
        builder.Property(x => x.LastModifiedAt).IsRequired(false);
    }
}
