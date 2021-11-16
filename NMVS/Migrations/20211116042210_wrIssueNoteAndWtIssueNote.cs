using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class wrIssueNoteAndWtIssueNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "SoIssueNoteDets",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackCount",
                table: "SoIssueNoteDets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WrIssueNoteDets",
                columns: table => new
                {
                    IndID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    PackCount = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    InId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrIssueNoteDets", x => x.IndID);
                });

            migrationBuilder.CreateTable(
                name: "WrIssueNotes",
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
                    table.PrimaryKey("PK_WrIssueNotes", x => x.InId);
                });

            migrationBuilder.CreateTable(
                name: "WtIssueNoteDets",
                columns: table => new
                {
                    IndID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    PackCount = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    InId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WtIssueNoteDets", x => x.IndID);
                });

            migrationBuilder.CreateTable(
                name: "WtIssueNotes",
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
                    table.PrimaryKey("PK_WtIssueNotes", x => x.InId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WrIssueNoteDets");

            migrationBuilder.DropTable(
                name: "WrIssueNotes");

            migrationBuilder.DropTable(
                name: "WtIssueNoteDets");

            migrationBuilder.DropTable(
                name: "WtIssueNotes");

            migrationBuilder.DropColumn(
                name: "PackCount",
                table: "SoIssueNoteDets");

            migrationBuilder.AlterColumn<string>(
                name: "Quantity",
                table: "SoIssueNoteDets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
