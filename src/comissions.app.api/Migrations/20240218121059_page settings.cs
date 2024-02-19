using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class pagesettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SocialMediaLinks",
                table: "UserArtists");

            migrationBuilder.RenameColumn(
                name: "Biography",
                table: "UserArtists",
                newName: "SocialMediaLink4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserArtists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserArtists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestGuidelines",
                table: "UserArtists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ArtistPageSettingsId",
                table: "UserArtists",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaLink1",
                table: "UserArtists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaLink2",
                table: "UserArtists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaLink3",
                table: "UserArtists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ArtistPageSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    RequestButtonHoverBGColor = table.Column<string>(type: "text", nullable: false),
                    RequestButtonTextColor = table.Column<string>(type: "text", nullable: false),
                    RequestButtonBGColor = table.Column<string>(type: "text", nullable: false),
                    RequestTermsColor = table.Column<string>(type: "text", nullable: false),
                    RequestBackgroundColor = table.Column<string>(type: "text", nullable: false),
                    RequestHeaderImageUrl = table.Column<string>(type: "text", nullable: false),
                    RequestHeaderUseImage = table.Column<bool>(type: "boolean", nullable: false),
                    RequestHeaderSize = table.Column<int>(type: "integer", nullable: false),
                    RequestHeaderColor = table.Column<string>(type: "text", nullable: false),
                    RequestHeaderText = table.Column<string>(type: "text", nullable: false),
                    PortfolioMaximumSize = table.Column<int>(type: "integer", nullable: false),
                    PortfolioEnabledScrolling = table.Column<bool>(type: "boolean", nullable: false),
                    PortfolioColumns = table.Column<int>(type: "integer", nullable: false),
                    PortfolioMasonry = table.Column<bool>(type: "boolean", nullable: false),
                    PortfolioBackgroundColor = table.Column<string>(type: "text", nullable: false),
                    PortfolionHeaderImageUrl = table.Column<string>(type: "text", nullable: false),
                    PortfolionHeaderUseImage = table.Column<bool>(type: "boolean", nullable: false),
                    PortfolionHeaderSize = table.Column<int>(type: "integer", nullable: false),
                    PortfolionHeaderColor = table.Column<string>(type: "text", nullable: false),
                    PortfolionHeaderText = table.Column<string>(type: "text", nullable: false),
                    DescriptionTextSize = table.Column<int>(type: "integer", nullable: false),
                    DescriptionTextColor = table.Column<string>(type: "text", nullable: false),
                    DescriptionBackgroundColor = table.Column<string>(type: "text", nullable: false),
                    DescriptionHeaderImageUrl = table.Column<string>(type: "text", nullable: false),
                    DescriptionHeaderUseImage = table.Column<bool>(type: "boolean", nullable: false),
                    DescriptionHeaderSize = table.Column<int>(type: "integer", nullable: false),
                    DescriptionHeaderColor = table.Column<string>(type: "text", nullable: false),
                    DescriptionHeaderText = table.Column<string>(type: "text", nullable: false),
                    HeaderImageUrl = table.Column<string>(type: "text", nullable: false),
                    HeaderUseImage = table.Column<bool>(type: "boolean", nullable: false),
                    HeaderTextSize = table.Column<int>(type: "integer", nullable: false),
                    HeaderColor = table.Column<string>(type: "text", nullable: false),
                    BackgroundColor = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistPageSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistPageSettings_UserArtists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "UserArtists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistPageSettings_ArtistId",
                table: "ArtistPageSettings",
                column: "ArtistId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistPageSettings");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "RequestGuidelines",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "ArtistPageSettingsId",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "SocialMediaLink1",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "SocialMediaLink2",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "SocialMediaLink3",
                table: "UserArtists");

            migrationBuilder.RenameColumn(
                name: "SocialMediaLink4",
                table: "UserArtists",
                newName: "Biography");

            migrationBuilder.AddColumn<List<string>>(
                name: "SocialMediaLinks",
                table: "UserArtists",
                type: "text[]",
                nullable: false);
        }
    }
}
