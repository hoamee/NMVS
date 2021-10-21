using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class AllocateORder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllocateOrders",
                columns: table => new
                {
                    AlcOrdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    ItemMasterPtId = table.Column<int>(type: "int", nullable: true),
                    AlcOrdQty = table.Column<double>(type: "float", nullable: false),
                    AlcOrdFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlcOrdFDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCode1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Confirm = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    AllocateRequestAlcId = table.Column<int>(type: "int", nullable: true),
                    OrderBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralizedCodeCodeNo = table.Column<int>(type: "int", nullable: true),
                    MovementTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocateOrders", x => x.AlcOrdId);
                    table.ForeignKey(
                        name: "FK_AllocateOrders_AllocateRequests_AllocateRequestAlcId",
                        column: x => x.AllocateRequestAlcId,
                        principalTable: "AllocateRequests",
                        principalColumn: "AlcId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllocateOrders_GeneralizedCodes_GeneralizedCodeCodeNo",
                        column: x => x.GeneralizedCodeCodeNo,
                        principalTable: "GeneralizedCodes",
                        principalColumn: "CodeNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllocateOrders_ItemMasters_ItemMasterPtId",
                        column: x => x.ItemMasterPtId,
                        principalTable: "ItemMasters",
                        principalColumn: "PtId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllocateOrders_Locs_LocCode1",
                        column: x => x.LocCode1,
                        principalTable: "Locs",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocateOrders_AllocateRequestAlcId",
                table: "AllocateOrders",
                column: "AllocateRequestAlcId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocateOrders_GeneralizedCodeCodeNo",
                table: "AllocateOrders",
                column: "GeneralizedCodeCodeNo");

            migrationBuilder.CreateIndex(
                name: "IX_AllocateOrders_ItemMasterPtId",
                table: "AllocateOrders",
                column: "ItemMasterPtId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocateOrders_LocCode1",
                table: "AllocateOrders",
                column: "LocCode1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllocateOrders");
        }
    }
}
