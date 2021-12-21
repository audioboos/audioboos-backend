using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Simplifyrefreshtokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_user_id",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "6eb20ce0-3d04-4c91-bd81-16cf59bcb573");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "8f211548-7410-4c96-b1e1-0dd1013b120e");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "ef88f817-12a1-4c56-9836-9e995eb1d9e6");

            migrationBuilder.DropColumn(
                name: "created",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "created_by_ip",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "expires",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "replaced_by_token",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "revoked",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "revoked_by_ip",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "app",
                table: "refresh_tokens",
                newName: "app_user_id");

            migrationBuilder.RenameColumn(
                name: "jwt",
                schema: "app",
                table: "refresh_tokens",
                newName: "jwt_token");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "app",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_app_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_jwt",
                schema: "app",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_jwt_token");

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

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token",
                schema: "app",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_users_app_user_id",
                schema: "app",
                table: "refresh_tokens",
                column: "app_user_id",
                principalSchema: "app",
                principalTable: "users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_users_app_user_id",
                schema: "app",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "ix_refresh_tokens_token",
                schema: "app",
                table: "refresh_tokens");

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

            migrationBuilder.RenameColumn(
                name: "jwt_token",
                schema: "app",
                table: "refresh_tokens",
                newName: "jwt");

            migrationBuilder.RenameColumn(
                name: "app_user_id",
                schema: "app",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_jwt_token",
                schema: "app",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_jwt");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_app_user_id",
                schema: "app",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_user_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                schema: "app",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "created_by_ip",
                schema: "app",
                table: "refresh_tokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "expires",
                schema: "app",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "replaced_by_token",
                schema: "app",
                table: "refresh_tokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "revoked",
                schema: "app",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "revoked_by_ip",
                schema: "app",
                table: "refresh_tokens",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "6eb20ce0-3d04-4c91-bd81-16cf59bcb573", "aa1ff998-4b62-46fc-9636-651b729ceae7", "Admin", "ADMIN" },
                    { "8f211548-7410-4c96-b1e1-0dd1013b120e", "86acffac-2522-4c5f-b816-690bb53cfc83", "Viewer", "VIEWER" },
                    { "ef88f817-12a1-4c56-9836-9e995eb1d9e6", "4db76b72-0c3b-4696-9be8-00406b101e97", "Editor", "EDITOR" }
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
    }
}
