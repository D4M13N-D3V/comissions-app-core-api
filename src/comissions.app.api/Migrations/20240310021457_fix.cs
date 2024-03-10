using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ArtistRequestId",
                table: "Requests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArtistRequestMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: false),
                    SentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistRequestMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistRequestMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ArtistRequestId",
                table: "Requests",
                column: "ArtistRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistRequestMessages_UserId",
                table: "ArtistRequestMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_ArtistRequests_ArtistRequestId",
                table: "Requests",
                column: "ArtistRequestId",
                principalTable: "ArtistRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_ArtistRequests_ArtistRequestId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "ArtistRequestMessages");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ArtistRequestId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ArtistRequestId",
                table: "Requests");

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
    }
}
