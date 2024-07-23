namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal class AssetConfiguration: IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Asset", t => t.HasComment("Installer asset."));

        builder.HasIndex(a => a.InstallerId);

        builder.HasIndex(a => a.OwnerId);

        builder.HasIndex(a => a.Name);

        builder.HasIndex(a => a.InstallationDateUtc);

        builder.Property(a => a.OwnerId)
            .HasComment("User ID of the consumer of the asset.");

        builder.Property(a => a.InstallerId)
            .HasComment("User ID of the asset's installer.");

        builder.OwnsOne(a => a.PricingDetails, builder =>
        {
            builder.ToTable("AssetPricingDetails");

            builder.HasIndex(p => p.PaymentPlan);
        });

        builder.OwnsOne(a => a.SolarSpecifications, builder =>
        {
            builder.ToTable("AssetSolarSpecifications");
        });

        builder.OwnsOne(a => a.InverterSpecifications, builder =>
        {
            builder.ToTable("AssetInverterSpecifications");
        });

        builder.OwnsOne(a => a.BatterySpecifications, builder =>
        {
            builder.ToTable("AssetBatterySpecifications");
        });
    }
}
