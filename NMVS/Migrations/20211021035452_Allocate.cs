using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class Allocate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllocateRequests",
                columns: table => new
                {
                    AlcId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    ItemMasterPtId = table.Column<int>(type: "int", nullable: true),
                    AlcQty = table.Column<double>(type: "float", nullable: false),
                    AlcFrom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AlcFromDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LocCode1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AlcCmmt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    MovementTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocateRequests", x => x.AlcId);
                    table.ForeignKey(
                        name: "FK_AllocateRequests_ItemMasters_ItemMasterPtId",
                        column: x => x.ItemMasterPtId,
                        principalTable: "ItemMasters",
                        principalColumn: "PtId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllocateRequests_Locs_LocCode1",
                        column: x => x.LocCode1,
                        principalTable: "Locs",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvRequests",
                columns: table => new
                {
                    RqID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CodeValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralizedCodeCodeNo = table.Column<int>(type: "int", nullable: true),
                    RqBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RqDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ref = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoConfirm = table.Column<bool>(type: "bit", nullable: false),
                    RqCmt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvRequests", x => x.RqID);
                    table.ForeignKey(
                        name: "FK_InvRequests_GeneralizedCodes_GeneralizedCodeCodeNo",
                        column: x => x.GeneralizedCodeCodeNo,
                        principalTable: "GeneralizedCodes",
                        principalColumn: "CodeNo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueOrders",
                columns: table => new
                {
                    ExpOrdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpOrdQty = table.Column<double>(type: "float", nullable: false),
                    WhlCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    ToLoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToLocDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Confirm = table.Column<bool>(type: "bit", nullable: true),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RqID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvRequestRqID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OrderBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MovementTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueOrders", x => x.ExpOrdId);
                    table.ForeignKey(
                        name: "FK_IssueOrders_InvRequests_InvRequestRqID",
                        column: x => x.InvRequestRqID,
                        principalTable: "InvRequests",
                        principalColumn: "RqID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueOrders_Locs_LocCode",
                        column: x => x.LocCode,
                        principalTable: "Locs",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestDets",
                columns: table => new
                {
                    DetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemDataItemNo = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Picked = table.Column<double>(type: "float", nullable: false),
                    Ready = table.Column<double>(type: "float", nullable: false),
                    Closed = table.Column<bool>(type: "bit", nullable: true),
                    Issued = table.Column<double>(type: "float", nullable: true),
                    RequireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Arranged = table.Column<double>(type: "float", nullable: false),
                    Shipped = table.Column<bool>(type: "bit", nullable: true),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpecDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RqID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvRequestRqID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDets", x => x.DetId);
                    table.ForeignKey(
                        name: "FK_RequestDets_InvRequests_InvRequestRqID",
                        column: x => x.InvRequestRqID,
                        principalTable: "InvRequests",
                        principalColumn: "RqID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestDets_ItemDatas_ItemDataItemNo",
                        column: x => x.ItemDataItemNo,
                        principalTable: "ItemDatas",
                        principalColumn: "ItemNo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocateRequests_ItemMasterPtId",
                table: "AllocateRequests",
                column: "ItemMasterPtId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocateRequests_LocCode1",
                table: "AllocateRequests",
                column: "LocCode1");

            migrationBuilder.CreateIndex(
                name: "IX_InvRequests_GeneralizedCodeCodeNo",
                table: "InvRequests",
                column: "GeneralizedCodeCodeNo");

            migrationBuilder.CreateIndex(
                name: "IX_IssueOrders_InvRequestRqID",
                table: "IssueOrders",
                column: "InvRequestRqID");

            migrationBuilder.CreateIndex(
                name: "IX_IssueOrders_LocCode",
                table: "IssueOrders",
                column: "LocCode");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDets_InvRequestRqID",
                table: "RequestDets",
                column: "InvRequestRqID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDets_ItemDataItemNo",
                table: "RequestDets",
                column: "ItemDataItemNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllocateRequests");

            migrationBuilder.DropTable(
                name: "IssueOrders");

            migrationBuilder.DropTable(
                name: "RequestDets");

            migrationBuilder.DropTable(
                name: "InvRequests");
        }
    }
}
