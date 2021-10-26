using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class prodLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProdLines",
                columns: table => new
                {
                    PrLnId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SiCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteSiCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdLines", x => x.PrLnId);
                    table.ForeignKey(
                        name: "FK_ProdLines_Sites_SiteSiCode",
                        column: x => x.SiteSiCode,
                        principalTable: "Sites",
                        principalColumn: "SiCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    WoNbr = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDataItemNo = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    QtyOrd = table.Column<double>(type: "float", nullable: false),
                    QtyCom = table.Column<double>(type: "float", nullable: false),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrdBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrLnId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProdLinePrLnId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Closed = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.WoNbr);
                    table.ForeignKey(
                        name: "FK_WorkOrders_ItemDatas_ItemDataItemNo",
                        column: x => x.ItemDataItemNo,
                        principalTable: "ItemDatas",
                        principalColumn: "ItemNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrders_ProdLines_ProdLinePrLnId",
                        column: x => x.ProdLinePrLnId,
                        principalTable: "ProdLines",
                        principalColumn: "PrLnId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WoBills",
                columns: table => new
                {
                    WoBillNbr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkOrderWoNbr = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OrdQty = table.Column<double>(type: "float", nullable: false),
                    ComQty = table.Column<double>(type: "float", nullable: false),
                    Assignee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reporter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WoBills", x => x.WoBillNbr);
                    table.ForeignKey(
                        name: "FK_WoBills_WorkOrders_WorkOrderWoNbr",
                        column: x => x.WorkOrderWoNbr,
                        principalTable: "WorkOrders",
                        principalColumn: "WoNbr",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdLines_SiteSiCode",
                table: "ProdLines",
                column: "SiteSiCode");

            migrationBuilder.CreateIndex(
                name: "IX_WoBills_WorkOrderWoNbr",
                table: "WoBills",
                column: "WorkOrderWoNbr");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ItemDataItemNo",
                table: "WorkOrders",
                column: "ItemDataItemNo");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ProdLinePrLnId",
                table: "WorkOrders",
                column: "ProdLinePrLnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WoBills");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "ProdLines");
        }
    }
}
