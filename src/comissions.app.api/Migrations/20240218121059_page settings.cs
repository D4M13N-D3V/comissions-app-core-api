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
                table: "UserSellerProfiles");

            migrationBuilder.RenameColumn(
                name: "Biography",
                table: "UserSellerProfiles",
                newName: "SocialMediaLink4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserSellerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserSellerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestGuidelines",
                table: "UserSellerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SellerProfilePageSettingsId",
                table: "UserSellerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaLink1",
                table: "UserSellerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaLink2",
                table: "UserSellerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SocialMediaLink3",
                table: "UserSellerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SellerProfilePageSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerProfileId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_SellerProfilePageSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerProfilePageSettings_UserSellerProfiles_SellerProfileId",
                        column: x => x.SellerProfileId,
                        principalTable: "UserSellerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellerProfilePageSettings_SellerProfileId",
                table: "SellerProfilePageSettings",
                column: "SellerProfileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellerProfilePageSettings");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "RequestGuidelines",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "SellerProfilePageSettingsId",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "SocialMediaLink1",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "SocialMediaLink2",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "SocialMediaLink3",
                table: "UserSellerProfiles");

            migrationBuilder.RenameColumn(
                name: "SocialMediaLink4",
                table: "UserSellerProfiles",
                newName: "Biography");

            migrationBuilder.AddColumn<List<string>>(
                name: "SocialMediaLinks",
                table: "UserSellerProfiles",
                type: "text[]",
                nullable: false);
        }
    }
}
