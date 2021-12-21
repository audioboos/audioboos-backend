using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Adduseridtorefreshtokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_app_user_id",
                schema: "app",
                table: "refresh_tokens");

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

            migrationBuilder.RenameColumn(
                name: "app_user_id",
                schema: "app",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_app_user_id",
                schema: "app",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_user_id");

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

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                schema: "app",
                table: "refresh_tokens",
                column: "user_id",
                principalSchema: "app",
                principalTable: "users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                schema: "app",
                table: "refresh_tokens");

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

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "app",
                table: "refresh_tokens",
                newName: "app_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "app",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_app_user_id");

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

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_app_user_id",
                schema: "app",
                table: "refresh_tokens",
                column: "app_user_id",
                principalSchema: "app",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
