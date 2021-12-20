using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Addedrefreshtokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "2e5dbaac-2235-48cd-b75a-76bf1302f7e0");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "603ed329-5c2b-4d3b-99ce-f5ccd84dbb7e");

            migrationBuilder.DeleteData(
                schema: "auth",
                table: "IdentityRole",
                keyColumn: "id",
                keyValue: "912392a7-bfbc-4ced-961f-2faef09c8b2a");

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_ip = table.Column<string>(type: "text", nullable: false),
                    revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_by_ip = table.Column<string>(type: "text", nullable: false),
                    replaced_by_token = table.Column<string>(type: "text", nullable: false),
                    app_user_id = table.Column<string>(type: "text", nullable: false)
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
                    { "2d3e4886-2221-4642-8269-8739e3357dfd", "e980406a-d17e-4e47-878c-d3a98fcffd6a", "Admin", "ADMIN" },
                    { "4fb4c2a6-169a-4f8b-814a-6e7d6fd2aa9b", "9d86273c-12ae-4d16-9736-20606c7cef14", "Viewer", "VIEWER" },
                    { "9346af1a-5109-4925-a3a5-33f5e44ba473", "7b6f0257-8f1e-4aa5-9d36-85f8947bc946", "Editor", "EDITOR" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_token_app_user_id",
                schema: "app",
                table: "RefreshToken",
                column: "app_user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken",
                schema: "app");

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

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "2e5dbaac-2235-48cd-b75a-76bf1302f7e0", "1c972aaa-3f26-4ca2-8c6c-f901ad7a2946", "Viewer", "VIEWER" },
                    { "603ed329-5c2b-4d3b-99ce-f5ccd84dbb7e", "9363a959-a325-4cdb-a743-c8172ae2b298", "Editor", "EDITOR" },
                    { "912392a7-bfbc-4ced-961f-2faef09c8b2a", "c7511f48-0884-4e2c-b348-56296a87190f", "Admin", "ADMIN" }
                });
        }
    }
}
