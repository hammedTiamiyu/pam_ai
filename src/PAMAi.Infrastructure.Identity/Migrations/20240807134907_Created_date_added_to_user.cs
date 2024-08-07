using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAMAi.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Created_date_added_to_user: Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedUtc",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: DateTimeOffset.Now);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "User");
        }
    }
}
