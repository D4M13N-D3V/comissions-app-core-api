using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class reviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Requests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewDate",
                table: "Requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewMessage",
                table: "Requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Reviewed",
                table: "Requests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ReviewDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ReviewMessage",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Reviewed",
                table: "Requests");
        }
    }
}
