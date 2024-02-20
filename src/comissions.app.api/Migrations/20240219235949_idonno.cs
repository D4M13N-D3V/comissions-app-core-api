using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class idonno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ArtistRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "ArtistRequests");
        }
    }
}
