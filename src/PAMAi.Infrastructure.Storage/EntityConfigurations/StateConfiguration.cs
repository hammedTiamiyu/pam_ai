namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal sealed class StateConfiguration: IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("State", t => t.HasComment("Country states."));

        builder.HasOne(s => s.Country)
            .WithMany(c => c.States)
            .HasForeignKey(s => s.CountryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
