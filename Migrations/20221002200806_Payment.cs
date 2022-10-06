using Microsoft.EntityFrameworkCore.Migrations;

namespace HamBurger.Migrations
{
    public partial class Payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CVC",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CardName",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpirationMonth",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExpirationYear",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVC",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CardName",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ExpirationMonth",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ExpirationYear",
                table: "OrderHeaders");
        }
    }
}
