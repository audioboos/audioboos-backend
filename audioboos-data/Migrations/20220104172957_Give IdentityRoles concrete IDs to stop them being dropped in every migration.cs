using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class GiveIdentityRolesconcreteIDstostopthembeingdroppedineverymigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "0f50900a-19dd-48d5-9aa9-e2f298ef4ef1", "4db8a34e-3084-4f5d-abdb-9da2265b824f", "Viewer", "VIEWER" },
                    { "ad46f0b2-8ced-4786-93af-0306ba77d522", "014638e8-fcff-4f9d-bc60-ce6ed0250965", "Editor", "EDITOR" },
                    { "b8e58349-3353-4aac-91a2-7146bb1a64cb", "0bd4d47e-8d86-4d75-a3ca-8dc175af4564", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "0f50900a-19dd-48d5-9aa9-e2f298ef4ef1");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "ad46f0b2-8ced-4786-93af-0306ba77d522");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "b8e58349-3353-4aac-91a2-7146bb1a64cb");

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
    }
}
