using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class forgotcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestButtonHoverTextColor",
                table: "ArtistPageSettings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestButtonHoverTextColor",
                table: "ArtistPageSettings");
        }
    }
}
