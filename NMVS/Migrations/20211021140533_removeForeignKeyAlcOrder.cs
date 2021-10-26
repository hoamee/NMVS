using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class removeForeignKeyAlcOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllocateOrders_AllocateRequests_AllocateRequestAlcId",
                table: "AllocateOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_AllocateOrders_GeneralizedCodes_GeneralizedCodeCodeNo",
                table: "AllocateOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_AllocateOrders_ItemMasters_ItemMasterPtId",
                table: "AllocateOrders");

            migrationBuilder.DropIndex(
                name: "IX_AllocateOrders_AllocateRequestAlcId",
                table: "AllocateOrders");

            migrationBuilder.DropIndex(
                name: "IX_AllocateOrders_GeneralizedCodeCodeNo",
                table: "AllocateOrders");

            migrationBuilder.DropIndex(
                name: "IX_AllocateOrders_ItemMasterPtId",
                table: "AllocateOrders");

            migrationBuilder.DropColumn(
                name: "AllocateRequestAlcId",
                table: "AllocateOrders");

            migrationBuilder.DropColumn(
                name: "CodeValue",
                table: "AllocateOrders");

            migrationBuilder.DropColumn(
                name: "GeneralizedCodeCodeNo",
                table: "AllocateOrders");

            migrationBuilder.DropColumn(
                name: "ItemMasterPtId",
                table: "AllocateOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllocateRequestAlcId",
                table: "AllocateOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeValue",
                table: "AllocateOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneralizedCodeCodeNo",
                table: "AllocateOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemMasterPtId",
                table: "AllocateOrders",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_AllocateOrders_AllocateRequests_AllocateRequestAlcId",
                table: "AllocateOrders",
                column: "AllocateRequestAlcId",
                principalTable: "AllocateRequests",
                principalColumn: "AlcId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AllocateOrders_GeneralizedCodes_GeneralizedCodeCodeNo",
                table: "AllocateOrders",
                column: "GeneralizedCodeCodeNo",
                principalTable: "GeneralizedCodes",
                principalColumn: "CodeNo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AllocateOrders_ItemMasters_ItemMasterPtId",
                table: "AllocateOrders",
                column: "ItemMasterPtId",
                principalTable: "ItemMasters",
                principalColumn: "PtId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
