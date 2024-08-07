using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAMAi.Infrastructure.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Terms_of_Service: Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TermsOfService",
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
                    table.PrimaryKey("PK_TermsOfService", x => x.Id);
                },
                comment: "Legal Terms of Service of the PAMAi application.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserTermsOfServiceConsent",
                columns: table => new
                {
                    UserProfileId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TermsOfServiceId = table.Column<int>(type: "int", nullable: false),
                    AcceptedDateUtc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTermsOfServiceConsent", x => new { x.TermsOfServiceId, x.UserProfileId });
                    table.ForeignKey(
                        name: "FK_UserTermsOfServiceConsent_TermsOfService_TermsOfServiceId",
                        column: x => x.TermsOfServiceId,
                        principalTable: "TermsOfService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTermsOfServiceConsent_UserProfile_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "User consent to the different terms and conditions in the application.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TermsOfService_Version",
                table: "TermsOfService",
                column: "Version",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTermsOfServiceConsent_AcceptedDateUtc",
                table: "UserTermsOfServiceConsent",
                column: "AcceptedDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_UserTermsOfServiceConsent_UserProfileId",
                table: "UserTermsOfServiceConsent",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTermsOfServiceConsent");

            migrationBuilder.DropTable(
                name: "TermsOfService");
        }
    }
}
