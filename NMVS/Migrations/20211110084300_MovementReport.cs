using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class MovementReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedTime",
                table: "IssueOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Reported",
                table: "IssueOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedTime",
                table: "AllocateRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Reported",
                table: "AllocateRequests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedTime",
                table: "AllocateOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovementNote",
                table: "AllocateOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Reported",
                table: "AllocateOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedTime",
                table: "IssueOrders");

            migrationBuilder.DropColumn(
                name: "Reported",
                table: "IssueOrders");

            migrationBuilder.DropColumn(
                name: "CompletedTime",
                table: "AllocateRequests");

            migrationBuilder.DropColumn(
                name: "Reported",
                table: "AllocateRequests");

            migrationBuilder.DropColumn(
                name: "CompletedTime",
                table: "AllocateOrders");

            migrationBuilder.DropColumn(
                name: "MovementNote",
                table: "AllocateOrders");

            migrationBuilder.DropColumn(
                name: "Reported",
                table: "AllocateOrders");
        }
    }
}
