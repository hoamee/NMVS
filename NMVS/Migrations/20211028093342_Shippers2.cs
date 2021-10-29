using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class Shippers2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShipperDets",
                columns: table => new
                {
                    SpDetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShpId = table.Column<int>(type: "int", nullable: false),
                    DetId = table.Column<int>(type: "int", nullable: false),
                    RqId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipperDets", x => x.SpDetId);
                });

            migrationBuilder.CreateTable(
                name: "Shippers",
                columns: table => new
                {
                    ShpId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShpDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShpFrom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShpTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShpVia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualIn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckInBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckOutBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Loc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shippers", x => x.ShpId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipperDets");

            migrationBuilder.DropTable(
                name: "Shippers");
        }
    }
}
