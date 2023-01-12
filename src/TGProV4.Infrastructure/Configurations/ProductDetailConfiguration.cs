namespace TGProV4.Infrastructure.Configurations;

public class ProductDetailConfiguration : IEntityTypeConfiguration<ProductDetail>
{
    public void Configure(EntityTypeBuilder<ProductDetail> builder)
    {
        builder.ToTable("ProductDetails", "Production");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Product)
               .WithMany(x => x.ProductDetails)
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Condition)
               .WithMany(x => x.Products)
               .HasForeignKey(x => x.ConditionId)
               .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Type)
               .WithMany(x => x.Products)
               .HasForeignKey(x => x.TypeId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.ProductId).IsRequired(false);
        builder.Property(x => x.ConditionId).IsRequired(false);
        builder.Property(x => x.TypeId).IsRequired(false);
        builder.Property(x => x.Sku).IsRequired().HasColumnType("nvarchar(50)");
        builder.Property(x => x.Specification).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.Description).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.Warranty).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.Price)
               .IsRequired()
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0m);
        builder.Property(x => x.OriginalPrice)
               .IsRequired()
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0m);
        builder.Property(x => x.UnitsInStock).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.UnitsOnOrder).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.Discontinued).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
        builder.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
        builder.Property(x => x.LastModifiedAt).IsRequired(false);
    }
}
