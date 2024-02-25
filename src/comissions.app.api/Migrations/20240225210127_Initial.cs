using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Biography = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    UserArtistId = table.Column<int>(type: "integer", nullable: true),
                    Banned = table.Column<bool>(type: "boolean", nullable: false),
                    BannedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnbanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BannedReason = table.Column<string>(type: "text", nullable: true),
                    BanAdminId = table.Column<string>(type: "text", nullable: true),
                    Suspended = table.Column<bool>(type: "boolean", nullable: false),
                    SuspendedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnsuspendDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuspendedReason = table.Column<string>(type: "text", nullable: true),
                    SuspendAdminId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtistRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false),
                    Reviewed = table.Column<bool>(type: "boolean", nullable: false),
                    Review = table.Column<string>(type: "text", nullable: true),
                    ReviewRating = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserArtists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RequestGuidelines = table.Column<string>(type: "text", nullable: false),
                    SocialMediaLink1 = table.Column<string>(type: "text", nullable: false),
                    SocialMediaLink2 = table.Column<string>(type: "text", nullable: false),
                    SocialMediaLink3 = table.Column<string>(type: "text", nullable: false),
                    SocialMediaLink4 = table.Column<string>(type: "text", nullable: false),
                    AgeRestricted = table.Column<bool>(type: "boolean", nullable: false),
                    StripeAccountId = table.Column<string>(type: "text", nullable: true),
                    PrepaymentRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Suspended = table.Column<bool>(type: "boolean", nullable: false),
                    SuspendedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnsuspendDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuspendedReason = table.Column<string>(type: "text", nullable: true),
                    SuspendAdminId = table.Column<string>(type: "text", nullable: true),
                    ArtistPageSettingsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArtists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserArtists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistPageSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    RequestButtonHoverBGColor = table.Column<string>(type: "text", nullable: false),
                    RequestButtonHoverTextColor = table.Column<string>(type: "text", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "ArtistPortfolioPieces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    FileReference = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistPortfolioPieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistPortfolioPieces_UserArtists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "UserArtists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Declined = table.Column<bool>(type: "boolean", nullable: false),
                    DeclinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentUrl = table.Column<string>(type: "text", nullable: true),
                    Paid = table.Column<bool>(type: "boolean", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_UserArtists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "UserArtists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    FileReference = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAssets_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestReferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    FileReference = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestReferences_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistPageSettings_ArtistId",
                table: "ArtistPageSettings",
                column: "ArtistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtistPortfolioPieces_ArtistId",
                table: "ArtistPortfolioPieces",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistRequests_UserId",
                table: "ArtistRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAssets_RequestId",
                table: "RequestAssets",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReferences_RequestId",
                table: "RequestReferences",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ArtistId",
                table: "Requests",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId",
                table: "Requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArtists_UserId",
                table: "UserArtists",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistPageSettings");

            migrationBuilder.DropTable(
                name: "ArtistPortfolioPieces");

            migrationBuilder.DropTable(
                name: "ArtistRequests");

            migrationBuilder.DropTable(
                name: "RequestAssets");

            migrationBuilder.DropTable(
                name: "RequestReferences");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "UserArtists");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
