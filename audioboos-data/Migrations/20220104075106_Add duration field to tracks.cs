using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Adddurationfieldtotracks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "duration",
                schema: "app",
                table: "tracks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "duration",
                schema: "app",
                table: "tracks");

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
    }
}
