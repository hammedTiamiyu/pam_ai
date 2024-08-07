namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal class UserTermsOfServiceConsentConfiguration: IEntityTypeConfiguration<UserTermsOfServiceConsent>
{
    public void Configure(EntityTypeBuilder<UserTermsOfServiceConsent> builder)
    {
        builder.ToTable("UserTermsOfServiceConsent", builder =>
        {
            builder.HasComment("User consents to the different terms and conditions in the application.");
        });

        builder.HasKey(u => new { u.UserProfileId, u.TermsOfServiceId });

        builder.HasIndex(u => u.AcceptedDateUtc);

        builder.Ignore(u => u.IsAccepted);
    }
}
