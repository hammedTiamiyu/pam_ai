using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAMAi.Infrastructure.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Country_Table: Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Company_CompanyId",
                table: "UserProfile");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_CompanyId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "UserProfile");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "UserProfile",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CompanyName",
                table: "UserProfile",
                column: "CompanyName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfile_CompanyName",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "UserProfile");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "UserProfile",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CompanyId",
                table: "UserProfile",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Company_CompanyId",
                table: "UserProfile",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
