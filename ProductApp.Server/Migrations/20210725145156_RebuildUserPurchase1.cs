using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductApp.Server.Migrations
{
    public partial class RebuildUserPurchase1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "UserProductBuy",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductPrice = table.Column<int>(type: "int", nullable: false),
                    ProductCoverPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    UserPurchaseId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProductBuy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProductBuy_UserPurchases_UserPurchaseId",
                        column: x => x.UserPurchaseId,
                        principalTable: "UserPurchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProductBuy_UserPurchaseId",
                table: "UserProductBuy",
                column: "UserPurchaseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProductBuy");

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
    }
}
