using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductApp.Server.Migrations
{
    public partial class RebuildUserPurchase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchases_UserCart_UserCartId",
                table: "UserPurchases");

            migrationBuilder.DropIndex(
                name: "IX_UserPurchases_UserCartId",
                table: "UserPurchases");

            migrationBuilder.DropColumn(
                name: "UserCartId",
                table: "UserPurchases");

            migrationBuilder.AddColumn<int>(
                name: "UserPurchaseId",
                table: "UserProductInCarts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProductInCarts_UserPurchaseId",
                table: "UserProductInCarts",
                column: "UserPurchaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProductInCarts_UserPurchases_UserPurchaseId",
                table: "UserProductInCarts",
                column: "UserPurchaseId",
                principalTable: "UserPurchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProductInCarts_UserPurchases_UserPurchaseId",
                table: "UserProductInCarts");

            migrationBuilder.DropIndex(
                name: "IX_UserProductInCarts_UserPurchaseId",
                table: "UserProductInCarts");

            migrationBuilder.DropColumn(
                name: "UserPurchaseId",
                table: "UserProductInCarts");

            migrationBuilder.AddColumn<string>(
                name: "UserCartId",
                table: "UserPurchases",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPurchases_UserCartId",
                table: "UserPurchases",
                column: "UserCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchases_UserCart_UserCartId",
                table: "UserPurchases",
                column: "UserCartId",
                principalTable: "UserCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
