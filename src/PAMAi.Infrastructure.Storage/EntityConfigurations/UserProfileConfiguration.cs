namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal sealed class UserProfileConfiguration: IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable(name: "UserProfile", t => t.HasComment("Profiles for users."));

        builder.HasIndex(u => new {u.FirstName, u.LastName}, "IX_UserProfile_Name");

        builder.Ignore(u => u.FullName);

        builder.HasIndex(u => u.UserId)
            .IsUnique(unique: true);

        builder.HasIndex(u => u.CompanyName);

        builder.HasOne(u => u.State)
            .WithMany()
            .HasForeignKey(u => u.StateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
