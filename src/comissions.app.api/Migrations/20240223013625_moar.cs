using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace comissions.app.api.Migrations
{
    /// <inheritdoc />
    public partial class moar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Requests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDate",
                table: "Requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "Requests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "PaidDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "Requests");
        }
    }
}
