
namespace PAMAi.Infrastructure.Storage.EntityConfigurations;

internal class TermsOfServiceConfiguration: IEntityTypeConfiguration<TermsOfService>
{
    public void Configure(EntityTypeBuilder<TermsOfService> builder)
    {
        builder.ToTable("TermsOfService", builder =>
        {
            builder.HasComment("Legal Terms of Service of the PAMAi application.");
        });

        builder.HasMany(l => l.UserTermsOfServiceConsents)
            .WithOne(u => u.TermsOfService)
            .HasForeignKey(u => u.TermsOfServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.Version)
            .IsUnique();

        builder.Ignore(l => l.IsActive);
    }
}
