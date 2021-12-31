using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Changetracksnavigationproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "1cfe32e1-b231-4187-97f9-042022c725d6");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "318483de-112c-4c80-83c7-95de5feec03a");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "33a7e0d3-7bf7-4546-8eb8-b248750cb0d1");

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "1dcd94ff-6aaf-4655-b8e5-41e302369dac", "5ef174ec-80e0-48ab-9cd9-44542bb7f0e0", "Admin", "ADMIN" },
                    { "bc750329-8733-4f8f-b342-e726ecfdd736", "8f169d12-b1c8-41c7-a197-55a315182d7e", "Editor", "EDITOR" },
                    { "dee8390a-0fbe-471a-9bc9-d5cdf39c6015", "ddde175b-4052-4f7e-a891-9737573d404d", "Viewer", "VIEWER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "1dcd94ff-6aaf-4655-b8e5-41e302369dac");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "bc750329-8733-4f8f-b342-e726ecfdd736");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "dee8390a-0fbe-471a-9bc9-d5cdf39c6015");

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "1cfe32e1-b231-4187-97f9-042022c725d6", "4f990493-4483-4c0e-9b2a-0548b0f0ea8a", "Admin", "ADMIN" },
                    { "318483de-112c-4c80-83c7-95de5feec03a", "b7b56f4e-f786-43ba-91fb-9cebf693b281", "Viewer", "VIEWER" },
                    { "33a7e0d3-7bf7-4546-8eb8-b248750cb0d1", "0984fcda-7be4-483e-8683-6948244865bd", "Editor", "EDITOR" }
                });
        }
    }
}
