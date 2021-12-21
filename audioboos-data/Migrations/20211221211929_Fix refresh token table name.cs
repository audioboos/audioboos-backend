using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Fixrefreshtokentablename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken",
                schema: "app");

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

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    jwt = table.Column<string>(type: "text", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_ip = table.Column<string>(type: "text", nullable: false),
                    revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_by_ip = table.Column<string>(type: "text", nullable: true),
                    replaced_by_token = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "app",
                        principalTable: "users",
                        principalColumn: "id");
                });

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

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_jwt",
                schema: "app",
                table: "refresh_tokens",
                column: "jwt",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "app",
                table: "refresh_tokens",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "app");

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

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    app_user_id = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_ip = table.Column<string>(type: "text", nullable: false),
                    expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    jwt = table.Column<string>(type: "text", nullable: false),
                    replaced_by_token = table.Column<string>(type: "text", nullable: true),
                    revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_by_ip = table.Column<string>(type: "text", nullable: true),
                    token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_token_users_app_user_id",
                        column: x => x.app_user_id,
                        principalSchema: "app",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "ix_refresh_token_app_user_id",
                schema: "app",
                table: "RefreshToken",
                column: "app_user_id");
        }
    }
}
