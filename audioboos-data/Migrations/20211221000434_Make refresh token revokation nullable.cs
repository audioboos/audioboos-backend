using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Makerefreshtokenrevokationnullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "revoked_by_ip",
                schema: "app",
                table: "RefreshToken",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "replaced_by_token",
                schema: "app",
                table: "RefreshToken",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "7e85aa22-14aa-4de3-9db5-e47b0cae23a8", "158ad59e-eda2-49c4-8099-bef09c740acb", "Editor", "EDITOR" },
                    { "90abf90d-31cd-4f89-8aa4-e2d0076f6ff3", "c2a8e512-2ddb-4238-9901-c73b88665d1d", "Admin", "ADMIN" },
                    { "9fb6cba8-de36-43d4-b7c4-507856440cd8", "2a5b63c1-c218-4e06-8314-27a5b0e76501", "Viewer", "VIEWER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "7e85aa22-14aa-4de3-9db5-e47b0cae23a8");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "90abf90d-31cd-4f89-8aa4-e2d0076f6ff3");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "9fb6cba8-de36-43d4-b7c4-507856440cd8");

            migrationBuilder.AlterColumn<string>(
                name: "revoked_by_ip",
                schema: "app",
                table: "RefreshToken",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "replaced_by_token",
                schema: "app",
                table: "RefreshToken",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

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
    }
}
