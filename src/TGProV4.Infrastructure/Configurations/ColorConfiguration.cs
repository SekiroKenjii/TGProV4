namespace TGProV4.Infrastructure.Configurations;

public class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        builder.ToTable("Colors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasColumnType("nvarchar(50)");
        builder.Property(x => x.Code).IsRequired().HasColumnType("nvarchar(30)");
        builder.Property(x => x.Type)
            .IsRequired()
            .HasColumnType("nvarchar(3)")
            .HasDefaultValue(ColorType.HEX.ToString());
        builder.Property(x => x.CreatedBy).IsRequired().HasColumnType("nvarchar(128)");
        builder.Property(x => x.CreatedAt).IsRequired().HasDefaultValue(DateTimeOffset.Now);
        builder.Property(x => x.LastModifiedBy).IsRequired(false).HasColumnType("nvarchar(128)");
        builder.Property(x => x.LastModifiedAt).IsRequired(false);
    }
}
