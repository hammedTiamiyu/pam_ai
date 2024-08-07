using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAMAi.Infrastructure.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Legal_Contract: Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Legal terms and conditions of the PAMAi application.");

            migrationBuilder.CreateTable(
                name: "LegalContract",
                schema: "Legal terms and conditions of the PAMAi application.",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Version = table.Column<double>(type: "double", nullable: false),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    EffectiveFromUtc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    DeactivatedUtc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalContract", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserLegalContractConsent",
                columns: table => new
                {
                    UserProfileId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LegalContractId = table.Column<int>(type: "int", nullable: false),
                    AcceptedDateUtc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLegalContractConsent", x => new { x.UserProfileId, x.LegalContractId });
                    table.ForeignKey(
                        name: "FK_UserLegalContractConsent_LegalContract_LegalContractId",
                        column: x => x.LegalContractId,
                        principalSchema: "Legal terms and conditions of the PAMAi application.",
                        principalTable: "LegalContract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLegalContractConsent_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LegalContract_Version",
                schema: "Legal terms and conditions of the PAMAi application.",
                table: "LegalContract",
                column: "Version",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLegalContractConsent_AcceptedDateUtc",
                table: "UserLegalContractConsent",
                column: "AcceptedDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserLegalContractConsent_LegalContractId",
                table: "UserLegalContractConsent",
                column: "LegalContractId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLegalContractConsent");

            migrationBuilder.DropTable(
                name: "LegalContract",
                schema: "Legal terms and conditions of the PAMAi application.");
        }
    }
}
