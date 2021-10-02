using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductApp.Server.Migrations
{
    public partial class RenameIsDeleted1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "UserProfiles",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "UserOrders",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "UserOrderProducts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "UserCreatedProducts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "PurchasesHistorys",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "Products",
                newName: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "UserProfiles",
                newName: "IsBlocked");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "UserOrders",
                newName: "IsBlocked");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "UserOrderProducts",
                newName: "IsBlocked");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "UserCreatedProducts",
                newName: "IsBlocked");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "PurchasesHistorys",
                newName: "IsBlocked");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Products",
                newName: "IsBlocked");
        }
    }
}
