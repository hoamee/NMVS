using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class InitIncoming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomingLists",
                columns: table => new
                {
                    IcId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierSupCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Po = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Vehicle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Driver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsWarranty = table.Column<bool>(type: "bit", nullable: false),
                    Closed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingLists", x => x.IcId);
                    table.ForeignKey(
                        name: "FK_IncomingLists_Suppliers_SupplierSupCode",
                        column: x => x.SupplierSupCode,
                        principalTable: "Suppliers",
                        principalColumn: "SupCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasters",
                columns: table => new
                {
                    PtId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PtLotNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PtQty = table.Column<double>(type: "float", nullable: false),
                    Accepted = table.Column<double>(type: "float", nullable: false),
                    Qc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtHold = table.Column<double>(type: "float", nullable: false),
                    LocCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtDateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierSupCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    RecQty = table.Column<double>(type: "float", nullable: false),
                    RecBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IcId = table.Column<int>(type: "int", nullable: false),
                    PtCmt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasters", x => x.PtId);
                    table.ForeignKey(
                        name: "FK_ItemMasters_IncomingLists_IcId",
                        column: x => x.IcId,
                        principalTable: "IncomingLists",
                        principalColumn: "IcId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemMasters_Suppliers_SupplierSupCode",
                        column: x => x.SupplierSupCode,
                        principalTable: "Suppliers",
                        principalColumn: "SupCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingLists_SupplierSupCode",
                table: "IncomingLists",
                column: "SupplierSupCode");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasters_IcId",
                table: "ItemMasters",
                column: "IcId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasters_SupplierSupCode",
                table: "ItemMasters",
                column: "SupplierSupCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemMasters");

            migrationBuilder.DropTable(
                name: "IncomingLists");
        }
    }
}
