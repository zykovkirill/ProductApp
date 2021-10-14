using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductApp.Server.Migrations
{
    public partial class ReD1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "UserOrderProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "UserOrderProducts");
        }
    }
}
