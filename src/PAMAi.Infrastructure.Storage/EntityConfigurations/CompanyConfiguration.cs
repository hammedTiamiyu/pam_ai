namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal sealed class CompanyConfiguration: IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");
    }
}
