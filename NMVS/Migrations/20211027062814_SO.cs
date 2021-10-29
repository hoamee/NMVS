using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class SO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    SoNbr = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerCustCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReqDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoCurr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipVia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Confirm = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.SoNbr);
                    table.ForeignKey(
                        name: "FK_SalesOrders_Customers_CustomerCustCode",
                        column: x => x.CustomerCustCode,
                        principalTable: "Customers",
                        principalColumn: "CustCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SoDetails",
                columns: table => new
                {
                    SodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesOrderSoNbr = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetPrice = table.Column<double>(type: "float", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    Avail = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoDetails", x => x.SodId);
                    table.ForeignKey(
                        name: "FK_SoDetails_SalesOrders_SalesOrderSoNbr",
                        column: x => x.SalesOrderSoNbr,
                        principalTable: "SalesOrders",
                        principalColumn: "SoNbr",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerCustCode",
                table: "SalesOrders",
                column: "CustomerCustCode");

            migrationBuilder.CreateIndex(
                name: "IX_SoDetails_SalesOrderSoNbr",
                table: "SoDetails",
                column: "SalesOrderSoNbr");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoDetails");

            migrationBuilder.DropTable(
                name: "SalesOrders");
        }
    }
}
