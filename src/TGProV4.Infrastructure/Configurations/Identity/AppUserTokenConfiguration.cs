namespace TGProV4.Infrastructure.Configurations.Identity;

public class AppUserTokenConfiguration : IEntityTypeConfiguration<AppUserToken>
{
    public void Configure(EntityTypeBuilder<AppUserToken> builder)
    {
        builder.ToTable("UserTokens", ApplicationConstants.TableSchemas.Identity);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
               .WithMany(y => y.UserTokens)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Expires).IsRequired();
        builder.Property(x => x.Revoked).IsRequired(false);
    }
}
