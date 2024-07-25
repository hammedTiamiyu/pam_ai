using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAMAi.Infrastructure.Storage.Migrations
{
    /// <inheritdoc />
    public partial class UserProfile_And_Asset_Redesign: Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Asset_InstallerId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_OwnerId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "InstallerId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Asset");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "UserProfile",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "UserProfile",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "InstallerProfileId",
                table: "Asset",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerProfileId",
                table: "Asset",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_Name",
                table: "UserProfile",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Asset_InstallerProfileId",
                table: "Asset",
                column: "InstallerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_OwnerProfileId",
                table: "Asset",
                column: "OwnerProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_UserProfile_InstallerProfileId",
                table: "Asset",
                column: "InstallerProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_UserProfile_OwnerProfileId",
                table: "Asset",
                column: "OwnerProfileId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_UserProfile_InstallerProfileId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_UserProfile_OwnerProfileId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_Name",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_Asset_InstallerProfileId",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_OwnerProfileId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "InstallerProfileId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "OwnerProfileId",
                table: "Asset");

            migrationBuilder.AddColumn<string>(
                name: "InstallerId",
                table: "Asset",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                comment: "User ID of the asset's installer.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Asset",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "",
                comment: "User ID of the consumer of the asset.")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_InstallerId",
                table: "Asset",
                column: "InstallerId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_OwnerId",
                table: "Asset",
                column: "OwnerId");
        }
    }
}
