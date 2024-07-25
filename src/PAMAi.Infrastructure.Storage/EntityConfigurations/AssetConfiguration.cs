namespace PAMAi.Infrastructure.Storage.EntityConfigurations;
internal class AssetConfiguration: IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Asset", t => t.HasComment("Installer asset."));

        builder.HasIndex(a => a.Name);

        builder.HasIndex(a => a.InstallationDateUtc);

        builder.HasOne(a => a.OwnerProfile)
            .WithMany()
            .HasForeignKey(a => a.OwnerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.InstallerProfile)
            .WithMany()
            .HasForeignKey(a => a.InstallerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

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
