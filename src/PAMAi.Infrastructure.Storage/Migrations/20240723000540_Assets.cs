using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAMAi.Infrastructure.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Assets: Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Location = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Size = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InstallationDateUtc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    GenerationLoad = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OwnerId = table.Column<string>(type: "varchar(255)", nullable: false, comment: "User ID of the consumer of the asset.")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InstallerId = table.Column<string>(type: "varchar(255)", nullable: false, comment: "User ID of the asset's installer.")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset", x => x.Id);
                },
                comment: "Installer asset.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssetBatterySpecifications",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TotalCapacity = table.Column<double>(type: "double", nullable: false),
                    UsableCapacity = table.Column<double>(type: "double", nullable: true),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Voltage = table.Column<double>(type: "double", nullable: false),
                    DepthOfDischarge = table.Column<double>(type: "double", nullable: true),
                    CycleLife = table.Column<int>(type: "int", nullable: true),
                    ChargeDischargeRate = table.Column<double>(type: "double", nullable: true),
                    Efficiency = table.Column<double>(type: "double", nullable: true),
                    ManagementSystem = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WarrantyAndLifespan = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetBatterySpecifications", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_AssetBatterySpecifications_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssetInverterSpecifications",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Capacity = table.Column<double>(type: "double", nullable: false),
                    Efficiency = table.Column<double>(type: "double", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InputVoltageLow = table.Column<double>(type: "double", nullable: false),
                    InputVoltageHigh = table.Column<double>(type: "double", nullable: false),
                    MpptChannels = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SystemDesign = table.Column<int>(type: "int", nullable: true),
                    CommunicationFeatures = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProtectionFeatures = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Certifications = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetInverterSpecifications", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_AssetInverterSpecifications_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssetPricingDetails",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PaymentPlan = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPricingDetails", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_AssetPricingDetails_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AssetSolarSpecifications",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PotentialShading = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    PanelType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Size = table.Column<int>(type: "int", nullable: false),
                    EfficiencyRating = table.Column<int>(type: "int", nullable: false),
                    PanelCount = table.Column<int>(type: "int", nullable: false),
                    PeakEnergyUsageTime = table.Column<int>(type: "int", nullable: true),
                    EnergyConsumptionHabit = table.Column<int>(type: "int", nullable: true),
                    EnergyGeneratedDaily = table.Column<double>(type: "double", nullable: true),
                    EnergyUsageFlexibility = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSolarSpecifications", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_AssetSolarSpecifications_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_InstallationDateUtc",
                table: "Asset",
                column: "InstallationDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_InstallerId",
                table: "Asset",
                column: "InstallerId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Name",
                table: "Asset",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_OwnerId",
                table: "Asset",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPricingDetails_PaymentPlan",
                table: "AssetPricingDetails",
                column: "PaymentPlan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetBatterySpecifications");

            migrationBuilder.DropTable(
                name: "AssetInverterSpecifications");

            migrationBuilder.DropTable(
                name: "AssetPricingDetails");

            migrationBuilder.DropTable(
                name: "AssetSolarSpecifications");

            migrationBuilder.DropTable(
                name: "Asset");
        }
    }
}
