using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Review",
                table: "ArtistRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ReviewRating",
                table: "ArtistRequests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Reviewed",
                table: "ArtistRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Review",
                table: "ArtistRequests");

            migrationBuilder.DropColumn(
                name: "ReviewRating",
                table: "ArtistRequests");

            migrationBuilder.DropColumn(
                name: "Reviewed",
                table: "ArtistRequests");
        }
    }
}
