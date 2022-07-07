using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ProductApp.Server.Migrations
{
    public partial class REDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCreatedProducts_UserProfiles_UserProfileId",
                table: "UserCreatedProducts");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCreatedProducts",
                table: "UserCreatedProducts");

            migrationBuilder.DropColumn(
                name: "ProductCount",
                table: "UserOrderProducts");

            migrationBuilder.DropColumn(
                name: "ProductCoverPath",
                table: "UserOrderProducts");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "UserOrderProducts");

            migrationBuilder.RenameTable(
                name: "UserCreatedProducts",
                newName: "BaseProduct");

            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "UserOrderProducts",
                newName: "Count");

            migrationBuilder.RenameIndex(
                name: "IX_UserCreatedProducts_UserProfileId",
                table: "BaseProduct",
                newName: "IX_BaseProduct_UserProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "UserOrderProducts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Y",
                table: "BaseProduct",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<float>(
                name: "X",
                table: "BaseProduct",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<float>(
                name: "Size",
                table: "BaseProduct",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BaseProduct",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "BaseProduct",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "BaseProduct",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseProduct",
                table: "BaseProduct",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrderProducts_ProductId",
                table: "UserOrderProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseProduct_UserProfiles_UserProfileId",
                table: "BaseProduct",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserOrderProducts_BaseProduct_ProductId",
                table: "UserOrderProducts",
                column: "ProductId",
                principalTable: "BaseProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseProduct_UserProfiles_UserProfileId",
                table: "BaseProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_UserOrderProducts_BaseProduct_ProductId",
                table: "UserOrderProducts");

            migrationBuilder.DropIndex(
                name: "IX_UserOrderProducts_ProductId",
                table: "UserOrderProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseProduct",
                table: "BaseProduct");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BaseProduct");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "BaseProduct");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "BaseProduct");

            migrationBuilder.RenameTable(
                name: "BaseProduct",
                newName: "UserCreatedProducts");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "UserOrderProducts",
                newName: "ProductPrice");

            migrationBuilder.RenameIndex(
                name: "IX_BaseProduct_UserProfileId",
                table: "UserCreatedProducts",
                newName: "IX_UserCreatedProducts_UserProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "UserOrderProducts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductCount",
                table: "UserOrderProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductCoverPath",
                table: "UserOrderProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "UserOrderProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Y",
                table: "UserCreatedProducts",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "X",
                table: "UserCreatedProducts",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Size",
                table: "UserCreatedProducts",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCreatedProducts",
                table: "UserCreatedProducts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CoverPath = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    ProductKind = table.Column<int>(type: "int", maxLength: 256, nullable: false),
                    ProductType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserCreatedProducts_UserProfiles_UserProfileId",
                table: "UserCreatedProducts",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
