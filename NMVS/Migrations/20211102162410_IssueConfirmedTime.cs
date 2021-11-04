using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class IssueConfirmedTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "IssueConfirmedTime",
                table: "Shippers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueConfirmedTime",
                table: "Shippers");
        }
    }
}
