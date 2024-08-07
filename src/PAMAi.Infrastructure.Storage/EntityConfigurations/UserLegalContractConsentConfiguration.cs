namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal class UserLegalContractConsentConfiguration: IEntityTypeConfiguration<UserLegalContractConsent>
{
    public void Configure(EntityTypeBuilder<UserLegalContractConsent> builder)
    {
        builder.ToTable("UserLegalContractConsent");

        builder.HasKey(u => new { u.UserProfileId, u.LegalContractId });

        builder.HasIndex(u => u.AcceptedDateUtc);

        builder.Ignore(u => u.IsAccepted);
    }
}
