using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class namingfixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistPortfolioPieces_ArtistServices_ArtistServiceId",
                table: "ArtistPortfolioPieces");

            migrationBuilder.DropTable(
                name: "ArtistServiceOrderReviews");

            migrationBuilder.DropTable(
                name: "ArtistServiceOrders");

            migrationBuilder.DropTable(
                name: "ArtistServices");

            migrationBuilder.DropIndex(
                name: "IX_ArtistPortfolioPieces_ArtistServiceId",
                table: "ArtistPortfolioPieces");

            migrationBuilder.DropColumn(
                name: "ArtistServiceId",
                table: "ArtistPortfolioPieces");

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

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ArtistId",
                table: "Requests",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId",
                table: "Requests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.AddColumn<int>(
                name: "ArtistServiceId",
                table: "ArtistPortfolioPieces",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArtistServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    Archived = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false)
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
                name: "ArtistServiceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtistId = table.Column<int>(type: "integer", nullable: false),
                    ArtistServiceId = table.Column<int>(type: "integer", nullable: false),
                    BuyerId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentUrl = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TermsAcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    ArtistServiceId = table.Column<int>(type: "integer", nullable: false),
                    ArtistServiceOrderId = table.Column<int>(type: "integer", nullable: false),
                    ReviewerId = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Review = table.Column<string>(type: "text", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "IX_ArtistPortfolioPieces_ArtistServiceId",
                table: "ArtistPortfolioPieces",
                column: "ArtistServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrderReviews_ArtistServiceId",
                table: "ArtistServiceOrderReviews",
                column: "ArtistServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrderReviews_ArtistServiceOrderId",
                table: "ArtistServiceOrderReviews",
                column: "ArtistServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrderReviews_ReviewerId",
                table: "ArtistServiceOrderReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrders_ArtistId",
                table: "ArtistServiceOrders",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrders_ArtistServiceId",
                table: "ArtistServiceOrders",
                column: "ArtistServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServiceOrders_BuyerId",
                table: "ArtistServiceOrders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistServices_ArtistId",
                table: "ArtistServices",
                column: "ArtistId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistPortfolioPieces_ArtistServices_ArtistServiceId",
                table: "ArtistPortfolioPieces",
                column: "ArtistServiceId",
                principalTable: "ArtistServices",
                principalColumn: "Id");
        }
    }
}
