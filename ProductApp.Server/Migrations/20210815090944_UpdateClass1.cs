using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductApp.Server.Migrations
{
    public partial class UpdateClass1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProductInCarts_UserCart_UserCartId",
                table: "UserProductInCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_UserDatas_UserProfileId",
                table: "UserProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchases_UserDatas_UserProfileId",
                table: "UserPurchases");

            migrationBuilder.DropTable(
                name: "UserCart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDatas",
                table: "UserDatas");

            migrationBuilder.RenameTable(
                name: "UserDatas",
                newName: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "UserCartId",
                table: "UserProductInCarts",
                newName: "UserOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProductInCarts_UserCartId",
                table: "UserProductInCarts",
                newName: "IX_UserProductInCarts_UserOrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserOrders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    TotalSum = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrders", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserProductInCarts_UserOrders_UserOrderId",
                table: "UserProductInCarts",
                column: "UserOrderId",
                principalTable: "UserOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_UserProfiles_UserProfileId",
                table: "UserProducts",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchases_UserProfiles_UserProfileId",
                table: "UserPurchases",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProductInCarts_UserOrders_UserOrderId",
                table: "UserProductInCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_UserProfiles_UserProfileId",
                table: "UserProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchases_UserProfiles_UserProfileId",
                table: "UserPurchases");

            migrationBuilder.DropTable(
                name: "UserOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                newName: "UserDatas");

            migrationBuilder.RenameColumn(
                name: "UserOrderId",
                table: "UserProductInCarts",
                newName: "UserCartId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProductInCarts_UserOrderId",
                table: "UserProductInCarts",
                newName: "IX_UserProductInCarts_UserCartId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDatas",
                table: "UserDatas",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserCart",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    TotalSum = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCart", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserProductInCarts_UserCart_UserCartId",
                table: "UserProductInCarts",
                column: "UserCartId",
                principalTable: "UserCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_UserDatas_UserProfileId",
                table: "UserProducts",
                column: "UserProfileId",
                principalTable: "UserDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchases_UserDatas_UserProfileId",
                table: "UserPurchases",
                column: "UserProfileId",
                principalTable: "UserDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
