using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductApp.Server.Migrations
{
    public partial class UpdateShema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchases_UserProfiles_UserProfileId",
                table: "UserPurchases");

            migrationBuilder.DropIndex(
                name: "IX_UserPurchases_UserProfileId",
                table: "UserPurchases");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "UserPurchases");

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "UserOrders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PurchasesHistorys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasesHistorys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserOrders_UserProfileId",
                table: "UserOrders",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrders_UserProfiles_UserProfileId",
                table: "UserOrders",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOrders_UserProfiles_UserProfileId",
                table: "UserOrders");

            migrationBuilder.DropTable(
                name: "PurchasesHistorys");

            migrationBuilder.DropIndex(
                name: "IX_UserOrders_UserProfileId",
                table: "UserOrders");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "UserOrders");

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "UserPurchases",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPurchases_UserProfileId",
                table: "UserPurchases",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchases_UserProfiles_UserProfileId",
                table: "UserPurchases",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
