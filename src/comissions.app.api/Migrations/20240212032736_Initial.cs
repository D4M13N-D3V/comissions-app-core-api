using System;
using System.Collections.Generic;
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
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false)
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
                    Biography = table.Column<string>(type: "text", nullable: false),
                    SocialMediaLinks = table.Column<List<string>>(type: "text[]", nullable: false),
                    AgeRestricted = table.Column<bool>(type: "boolean", nullable: false),
                    StripeAccountId = table.Column<string>(type: "text", nullable: true),
                    PrepaymentRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Suspended = table.Column<bool>(type: "boolean", nullable: false),
                    SuspendedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnsuspendDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuspendedReason = table.Column<string>(type: "text", nullable: true),
                    SuspendAdminId = table.Column<string>(type: "text", nullable: true)
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
                name: "ArtistServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Archived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistServices_UserArtists_ArtistId",
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
                    FileReference = table.Column<string>(type: "text", nullable: false),
                    ArtistServiceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistPortfolioPieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistPortfolioPieces_ArtistServices_ArtistServiceId",
                        column: x => x.ArtistServiceId,
                        principalTable: "ArtistServices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArtistPortfolioPieces_UserArtists_ArtistProfi~",
                        column: x => x.ArtistId,
                        principalTable: "UserArtists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistServiceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BuyerId = table.Column<string>(type: "text", nullable: false),
                    ArtistServiceId = table.Column<int>(type: "integer", nullable: false),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TermsAcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistServiceOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistServiceOrders_ArtistServices_ArtistServiceId",
                        column: x => x.ArtistServiceId,
                        principalTable: "ArtistServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistServiceOrders_UserArtists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "UserArtists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistServiceOrders_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtistServiceOrderReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewerId = table.Column<string>(type: "text", nullable: false),
                    ArtistServiceOrderId = table.Column<int>(type: "integer", nullable: false),
                    ArtistServiceId = table.Column<int>(type: "integer", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Review = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistServiceOrderReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistServiceOrderReviews_ArtistServiceOrders_ArtistService~",
                        column: x => x.ArtistServiceOrderId,
                        principalTable: "ArtistServiceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistServiceOrderReviews_ArtistServices_ArtistServiceId",
                        column: x => x.ArtistServiceId,
                        principalTable: "ArtistServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistServiceOrderReviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistPortfolioPieces_ArtistId",
                table: "ArtistPortfolioPieces",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistPortfolioPieces_ArtistServiceId",
                table: "ArtistPortfolioPieces",
                column: "ArtistServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistRequests_UserId",
                table: "ArtistRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrderReviews_ReviewerId",
                table: "ArtistServiceOrderReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrderReviews_ArtistServiceId",
                table: "ArtistServiceOrderReviews",
                column: "ArtistServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrderReviews_ArtistServiceOrderId",
                table: "ArtistServiceOrderReviews",
                column: "ArtistServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrders_BuyerId",
                table: "ArtistServiceOrders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrders_ArtistId",
                table: "ArtistServiceOrders",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrders_ArtistServiceId",
                table: "ArtistServiceOrders",
                column: "ArtistServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServices_ArtistId",
                table: "ArtistServices",
                column: "ArtistId");

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
                name: "ArtistPortfolioPieces");

            migrationBuilder.DropTable(
                name: "ArtistRequests");

            migrationBuilder.DropTable(
                name: "ArtistServiceOrderReviews");

            migrationBuilder.DropTable(
                name: "ArtistServiceOrders");

            migrationBuilder.DropTable(
                name: "ArtistServices");

            migrationBuilder.DropTable(
                name: "UserArtists");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
