using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class soIssueNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoIssueNoteDets",
                columns: table => new
                {
                    IndID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoIssueNoteDets", x => x.IndID);
                });

            migrationBuilder.CreateTable(
                name: "SoIssueNotes",
                columns: table => new
                {
                    InId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shipper = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoIssueNotes", x => x.InId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoIssueNoteDets");

            migrationBuilder.DropTable(
                name: "SoIssueNotes");
        }
    }
}
