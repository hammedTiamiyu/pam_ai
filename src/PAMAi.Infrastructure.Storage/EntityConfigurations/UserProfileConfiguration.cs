namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal sealed class UserProfileConfiguration: IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable(name: "UserProfile", t =>
        {
            t.HasComment("Profiles for users.");
            //t.HasCheckConstraint("CK_UserProfile_UserId", "UserId IN (SELECT Id FROM User)");
        });

        builder.HasIndex(u => u.UserId)
            .IsUnique(unique: true);

        builder.HasOne(u => u.State)
            .WithMany()
            .HasForeignKey(u => u.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Company)
            .WithMany(c => c.UserProfiles)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
