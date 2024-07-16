namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal sealed class CountryConfiguration: IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Country");
    }
}
