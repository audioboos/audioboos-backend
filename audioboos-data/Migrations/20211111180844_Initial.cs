﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "artists",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    style = table.Column<string>(type: "text", nullable: true),
                    genre = table.Column<string>(type: "text", nullable: true),
                    small_image = table.Column<string>(type: "text", nullable: true),
                    large_image = table.Column<string>(type: "text", nullable: true),
                    header_image = table.Column<string>(type: "text", nullable: true),
                    fanart = table.Column<string>(type: "text", nullable: true),
                    music_brainz_id = table.Column<string>(type: "text", nullable: true),
                    discogs_id = table.Column<string>(type: "text", nullable: true),
                    aliases = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    tagging_status = table.Column<int>(type: "integer", nullable: false),
                    alternative_names = table.Column<string>(type: "text", nullable: false),
                    first_scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                schema: "app",
                columns: table => new
                {
                    key = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    release_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    site_id = table.Column<string>(type: "text", nullable: true),
                    small_image = table.Column<string>(type: "text", nullable: true),
                    large_image = table.Column<string>(type: "text", nullable: true),
                    artist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    tagging_status = table.Column<int>(type: "integer", nullable: false),
                    alternative_names = table.Column<string>(type: "text", nullable: false),
                    first_scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_albums", x => x.id);
                    table.ForeignKey(
                        name: "fk_albums_artists_artist_id",
                        column: x => x.artist_id,
                        principalSchema: "app",
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoleClaim",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_identity_role_claim_identity_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth",
                        principalTable: "IdentityRole",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserClaim",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_identity_user_claim_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "app",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserLogin",
                schema: "auth",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_login", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_identity_user_login_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "app",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserRole",
                schema: "auth",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_identity_user_role_identity_role_role_id",
                        column: x => x.role_id,
                        principalSchema: "auth",
                        principalTable: "IdentityRole",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_identity_user_role_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "app",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserToken",
                schema: "auth",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    login_provider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_user_token", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_identity_user_token_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "app",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tracks",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    track_number = table.Column<int>(type: "integer", nullable: false),
                    comments = table.Column<string>(type: "text", nullable: true),
                    audio_url = table.Column<string>(type: "text", nullable: true),
                    physical_path = table.Column<string>(type: "text", nullable: false),
                    album_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    tagging_status = table.Column<int>(type: "integer", nullable: false),
                    alternative_names = table.Column<string>(type: "text", nullable: false),
                    first_scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracks", x => x.id);
                    table.ForeignKey(
                        name: "fk_tracks_albums_album_id",
                        column: x => x.album_id,
                        principalSchema: "app",
                        principalTable: "albums",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audio_files",
                schema: "app",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    physical_path = table.Column<string>(type: "text", nullable: false),
                    id3artist_name = table.Column<string>(type: "text", nullable: false),
                    id3album_name = table.Column<string>(type: "text", nullable: false),
                    id3track_name = table.Column<string>(type: "text", nullable: false),
                    artist_id = table.Column<Guid>(type: "uuid", nullable: true),
                    album_id = table.Column<Guid>(type: "uuid", nullable: true),
                    track_id = table.Column<Guid>(type: "uuid", nullable: true),
                    checksum = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    tagging_status = table.Column<int>(type: "integer", nullable: false),
                    alternative_names = table.Column<string>(type: "text", nullable: false),
                    first_scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_scan_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audio_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_audio_files_albums_album_id",
                        column: x => x.album_id,
                        principalSchema: "app",
                        principalTable: "albums",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_audio_files_artists_artist_id",
                        column: x => x.artist_id,
                        principalSchema: "app",
                        principalTable: "artists",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_audio_files_tracks_track_id",
                        column: x => x.track_id,
                        principalSchema: "app",
                        principalTable: "tracks",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "5bdc02fb-49ee-4b18-b704-95e814dad1a3", "e03a0f2b-d6d9-4f97-9b2e-6515f8132d86", "Admin", "ADMIN" },
                    { "9d402f75-fa1e-4ed7-b3ba-ad7bbdcfb4bb", "8d573fbd-177a-4a86-a7d5-06e4c26d9e3b", "Editor", "EDITOR" },
                    { "df301601-9b70-4e3b-b1a4-3b36a06ead43", "75ead5da-d156-484d-bbf7-b29f1b05823a", "Viewer", "VIEWER" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_albums_artist_id_name",
                schema: "app",
                table: "albums",
                columns: new[] { "artist_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_artists_name",
                schema: "app",
                table: "artists",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_audio_files_album_id",
                schema: "app",
                table: "audio_files",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_audio_files_artist_id",
                schema: "app",
                table: "audio_files",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_audio_files_physical_path",
                schema: "app",
                table: "audio_files",
                column: "physical_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_audio_files_track_id",
                schema: "app",
                table: "audio_files",
                column: "track_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "auth",
                table: "IdentityRole",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identity_role_claim_role_id",
                schema: "auth",
                table: "IdentityRoleClaim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_claim_user_id",
                schema: "auth",
                table: "IdentityUserClaim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_login_user_id",
                schema: "auth",
                table: "IdentityUserLogin",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_identity_user_role_role_id",
                schema: "auth",
                table: "IdentityUserRole",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_settings_key",
                schema: "app",
                table: "settings",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tracks_album_id",
                schema: "app",
                table: "tracks",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "ix_tracks_physical_path",
                schema: "app",
                table: "tracks",
                column: "physical_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "app",
                table: "users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "app",
                table: "users",
                column: "normalized_user_name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audio_files",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IdentityRoleClaim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "IdentityUserClaim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "IdentityUserLogin",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "IdentityUserRole",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "IdentityUserToken",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "settings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "tracks",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IdentityRole",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "users",
                schema: "app");

            migrationBuilder.DropTable(
                name: "albums",
                schema: "app");

            migrationBuilder.DropTable(
                name: "artists",
                schema: "app");
        }
    }
}
