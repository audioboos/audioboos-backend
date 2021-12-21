using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class RefreshTokenFieldName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "2d3e4886-2221-4642-8269-8739e3357dfd");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "4fb4c2a6-169a-4f8b-814a-6e7d6fd2aa9b");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "9346af1a-5109-4925-a3a5-33f5e44ba473");

            migrationBuilder.AddColumn<string>(
                name: "jwt",
                schema: "app",
                table: "RefreshToken",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "0ff1859e-b6b5-4058-9791-2415b7e0279d", "6a2238bd-db07-4cbe-aafa-1466e1038e5c", "Editor", "EDITOR" },
                    { "9068ccec-bd02-4bde-97f8-5203a85eb94f", "49cf9756-a2bf-4c37-85c0-3802c467ea4d", "Admin", "ADMIN" },
                    { "9e77b18a-cffd-4170-8350-aa15c23054a4", "3e8be8a9-b8c3-46da-bfe0-0c747f0ecc62", "Viewer", "VIEWER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "0ff1859e-b6b5-4058-9791-2415b7e0279d");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "9068ccec-bd02-4bde-97f8-5203a85eb94f");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "9e77b18a-cffd-4170-8350-aa15c23054a4");

            migrationBuilder.DropColumn(
                name: "jwt",
                schema: "app",
                table: "RefreshToken");

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "2d3e4886-2221-4642-8269-8739e3357dfd", "e980406a-d17e-4e47-878c-d3a98fcffd6a", "Admin", "ADMIN" },
                    { "4fb4c2a6-169a-4f8b-814a-6e7d6fd2aa9b", "9d86273c-12ae-4d16-9736-20606c7cef14", "Viewer", "VIEWER" },
                    { "9346af1a-5109-4925-a3a5-33f5e44ba473", "7b6f0257-8f1e-4aa5-9d36-85f8947bc946", "Editor", "EDITOR" }
                });
        }
    }
}
