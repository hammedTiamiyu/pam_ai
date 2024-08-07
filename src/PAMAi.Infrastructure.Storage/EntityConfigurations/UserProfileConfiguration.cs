namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal sealed class UserProfileConfiguration: IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable(name: "UserProfile", t => t.HasComment("Profiles for users."));

        builder.HasIndex(u => new { u.FirstName, u.LastName }, "IX_UserProfile_Name");

        builder.Ignore(u => u.FullName);

        builder.HasIndex(u => u.UserId)
            .IsUnique(unique: true);

        builder.HasIndex(u => u.CompanyName);

        builder.HasOne(u => u.State)
            .WithMany()
            .HasForeignKey(u => u.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.TermsOfService)
            .WithMany(t => t.UserProfiles)
            .UsingEntity<UserTermsOfServiceConsent>(builder =>
            {
                builder.ToTable("UserTermsOfServiceConsent", t =>
                {
                    t.HasComment("User consent to the different terms and conditions in the application.");
                });

                builder.HasIndex(ut => ut.AcceptedDateUtc);

                builder.Ignore(ut => ut.IsAccepted);
            });
    }
}
