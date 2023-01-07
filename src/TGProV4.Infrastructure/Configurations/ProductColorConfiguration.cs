namespace TGProV4.Infrastructure.Configurations;

public class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
{
    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        builder.ToTable("ProductColors");

        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Product).WithMany(x => x.Colors)
            .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Color).WithMany(x => x.ProductColors)
            .HasForeignKey(x => x.ColorId).OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(x => x.ProductId).IsRequired(false);
        builder.Property(x => x.ColorId).IsRequired(false);
    }
}
