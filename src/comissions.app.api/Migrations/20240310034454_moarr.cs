using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class moarr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_ArtistRequests_ArtistRequestId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ArtistRequestId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "BanAdminId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Banned",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspendAdminId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Suspended",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspendedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspendedReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnbanDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnsuspendDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspendAdminId",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "Suspended",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "SuspendedDate",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "SuspendedReason",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "UnsuspendDate",
                table: "UserArtists");

            migrationBuilder.DropColumn(
                name: "ArtistRequestId",
                table: "Requests");

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ArtistRequestId",
                table: "ArtistRequestMessages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    BanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnbanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Voided = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suspensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SuspensionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnsuspensionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Voided = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suspensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suspensions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistRequestMessages_ArtistRequestId",
                table: "ArtistRequestMessages",
                column: "ArtistRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UserId",
                table: "Bans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Suspensions_UserId",
                table: "Suspensions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistRequestMessages_ArtistRequests_ArtistRequestId",
                table: "ArtistRequestMessages",
                column: "ArtistRequestId",
                principalTable: "ArtistRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistRequestMessages_ArtistRequests_ArtistRequestId",
                table: "ArtistRequestMessages");

            migrationBuilder.DropTable(
                name: "Bans");

            migrationBuilder.DropTable(
                name: "Suspensions");

            migrationBuilder.DropIndex(
                name: "IX_ArtistRequestMessages_ArtistRequestId",
                table: "ArtistRequestMessages");

            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ArtistRequestId",
                table: "ArtistRequestMessages");

            migrationBuilder.AddColumn<string>(
                name: "BanAdminId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Banned",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannedReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendAdminId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Suspended",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendedReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnbanDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnsuspendDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendAdminId",
                table: "UserArtists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Suspended",
                table: "UserArtists",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedDate",
                table: "UserArtists",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendedReason",
                table: "UserArtists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnsuspendDate",
                table: "UserArtists",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArtistRequestId",
                table: "Requests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ArtistRequestId",
                table: "Requests",
                column: "ArtistRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_ArtistRequests_ArtistRequestId",
                table: "Requests",
                column: "ArtistRequestId",
                principalTable: "ArtistRequests",
                principalColumn: "Id");
        }
    }
}
