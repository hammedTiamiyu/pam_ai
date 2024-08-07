
namespace PAMAi.Infrastructure.Storage.EntityConfigurations;

internal class LegalContractConfiguration: IEntityTypeConfiguration<LegalContract>
{
    public void Configure(EntityTypeBuilder<LegalContract> builder)
    {
        builder.ToTable("LegalContract", "Legal terms and conditions of the PAMAi application.");

        builder.HasMany(l => l.UserLegalContractConsents)
            .WithOne(u => u.LegalContract)
            .HasForeignKey(u => u.LegalContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.Version)
            .IsUnique();

        builder.Ignore(l => l.IsActive);
    }
}
