using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductApp.Server.Migrations
{
    public partial class AddDB2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverPath",
                table: "UserProducts",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPath",
                table: "UserProducts");
        }
    }
}
