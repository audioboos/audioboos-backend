using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Addedgenrestoalbum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "649ffdd9-023f-4228-853c-96053bfd96b2");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "95fabd8a-88de-4a3a-8fc2-8a949a3423c5");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "be682db4-0e3a-4e8a-a18f-21194f180d90");

            migrationBuilder.AddColumn<List<string>>(
                name: "genres",
                schema: "app",
                table: "albums",
                type: "text[]",
                nullable: true);

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "146ed38d-448c-4465-915b-eaca7b05a221", "45c86771-1ff9-49b5-8555-56c345b182ac", "Editor", "EDITOR" },
                    { "c503ed6a-b1ee-4496-be56-8de28ae19148", "7bc3ea27-a60c-4447-bfd1-3d229b9b5df9", "Viewer", "VIEWER" },
                    { "e5a7cb5f-12b6-4f5d-97bc-7051f2edb19e", "cc5a5075-1700-487d-bf33-03d951ef2792", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "146ed38d-448c-4465-915b-eaca7b05a221");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "c503ed6a-b1ee-4496-be56-8de28ae19148");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "e5a7cb5f-12b6-4f5d-97bc-7051f2edb19e");

            migrationBuilder.DropColumn(
                name: "genres",
                schema: "app",
                table: "albums");

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "649ffdd9-023f-4228-853c-96053bfd96b2", "e9c4abf1-5287-4cf3-928a-8142d71b688c", "Viewer", "VIEWER" },
                    { "95fabd8a-88de-4a3a-8fc2-8a949a3423c5", "602b127d-a9e8-40b1-9e27-7d6ca423e14e", "Admin", "ADMIN" },
                    { "be682db4-0e3a-4e8a-a18f-21194f180d90", "8fd4e21c-adc7-4fa6-93c6-7a31e2be6594", "Editor", "EDITOR" }
                });
        }
    }
}
