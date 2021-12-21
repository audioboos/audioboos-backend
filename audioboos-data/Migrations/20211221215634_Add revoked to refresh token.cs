using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Addrevokedtorefreshtoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "0469b418-74c0-4ea0-a421-ca9823167212");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "0ce8c176-b913-4ef8-bb7c-b2a8a83ab074");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "303f7a2f-b373-48bd-bb3a-eafbe8ac15c9");

            migrationBuilder.AddColumn<bool>(
                name: "revoked",
                schema: "app",
                table: "refresh_tokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "18c88687-4782-47cc-82cc-b20f591217bc", "8e24102c-b9f9-4621-9273-67ac04634c7f", "Editor", "EDITOR" },
                    { "3ee7626c-aa24-4430-8bba-2f8586546bce", "6b25c394-53f1-44ea-93c9-2759d4984120", "Admin", "ADMIN" },
                    { "538b0c6c-9a4b-44a8-ab4a-b31903b7e4a4", "bd3e4409-0f5e-4d83-8565-b6eb55d529c4", "Viewer", "VIEWER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "18c88687-4782-47cc-82cc-b20f591217bc");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "3ee7626c-aa24-4430-8bba-2f8586546bce");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "538b0c6c-9a4b-44a8-ab4a-b31903b7e4a4");

            migrationBuilder.DropColumn(
                name: "revoked",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "0469b418-74c0-4ea0-a421-ca9823167212", "d9f1b0b3-6f89-4e65-875c-914a13872e06", "Editor", "EDITOR" },
                    { "0ce8c176-b913-4ef8-bb7c-b2a8a83ab074", "8a3801a0-9110-4e3a-a9aa-2aa1bce7135f", "Admin", "ADMIN" },
                    { "303f7a2f-b373-48bd-bb3a-eafbe8ac15c9", "7309c64a-5441-47d7-b931-2aea49af5b65", "Viewer", "VIEWER" }
                });
        }
    }
}
