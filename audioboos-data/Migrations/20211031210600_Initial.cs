using System;
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
                name: "AppUsers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Artists",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Style = table.Column<string>(type: "text", nullable: true),
                    Genre = table.Column<string>(type: "text", nullable: true),
                    SmallImage = table.Column<string>(type: "text", nullable: true),
                    LargeImage = table.Column<string>(type: "text", nullable: true),
                    HeaderImage = table.Column<string>(type: "text", nullable: true),
                    Fanart = table.Column<string>(type: "text", nullable: true),
                    MusicBrainzId = table.Column<string>(type: "text", nullable: true),
                    DiscogsId = table.Column<string>(type: "text", nullable: true),
                    Aliases = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TaggingStatus = table.Column<int>(type: "integer", nullable: false),
                    AlternativeNames = table.Column<string>(type: "text", nullable: false),
                    FirstScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserClaim",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserLogin",
                schema: "auth",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserToken",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_IdentityUserToken_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SiteId = table.Column<string>(type: "text", nullable: true),
                    SmallImage = table.Column<string>(type: "text", nullable: true),
                    LargeImage = table.Column<string>(type: "text", nullable: true),
                    ArtistId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TaggingStatus = table.Column<int>(type: "integer", nullable: false),
                    AlternativeNames = table.Column<string>(type: "text", nullable: false),
                    FirstScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalSchema: "app",
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRoleClaim",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaim_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserRole",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUserRole_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "IdentityRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackNumber = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    AudioUrl = table.Column<string>(type: "text", nullable: true),
                    PhysicalPath = table.Column<string>(type: "text", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TaggingStatus = table.Column<int>(type: "integer", nullable: false),
                    AlternativeNames = table.Column<string>(type: "text", nullable: false),
                    FirstScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tracks_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalSchema: "app",
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AudioFiles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhysicalPath = table.Column<string>(type: "text", nullable: false),
                    ID3ArtistName = table.Column<string>(type: "text", nullable: false),
                    ID3AlbumName = table.Column<string>(type: "text", nullable: false),
                    ID3TrackName = table.Column<string>(type: "text", nullable: false),
                    ArtistId = table.Column<Guid>(type: "uuid", nullable: true),
                    AlbumId = table.Column<Guid>(type: "uuid", nullable: true),
                    TrackId = table.Column<Guid>(type: "uuid", nullable: true),
                    Checksum = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TaggingStatus = table.Column<int>(type: "integer", nullable: false),
                    AlternativeNames = table.Column<string>(type: "text", nullable: false),
                    FirstScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastScanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioFiles_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalSchema: "app",
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AudioFiles_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalSchema: "app",
                        principalTable: "Artists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AudioFiles_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalSchema: "app",
                        principalTable: "Tracks",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4deda126-ddcc-465c-8a07-34a0573ffdff", "bf7e08f9-a913-4251-8245-e977cb460e0e", "Admin", "ADMIN" },
                    { "8527b8ba-9e00-437c-98a4-f2dc4ece392d", "b78de54b-ba55-4dbf-9709-7103da9d96a3", "Editor", "EDITOR" },
                    { "d9dbdcff-2849-42f9-80b0-826241ed21a7", "80893c90-2f6c-4021-ba09-260c66069fae", "Viewer", "VIEWER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId_Name",
                schema: "app",
                table: "Albums",
                columns: new[] { "ArtistId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "app",
                table: "AppUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "app",
                table: "AppUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_Name",
                schema: "app",
                table: "Artists",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_AlbumId",
                schema: "app",
                table: "AudioFiles",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_ArtistId",
                schema: "app",
                table: "AudioFiles",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_PhysicalPath",
                schema: "app",
                table: "AudioFiles",
                column: "PhysicalPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_TrackId",
                schema: "app",
                table: "AudioFiles",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "auth",
                table: "IdentityRole",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityRoleClaim_RoleId",
                schema: "auth",
                table: "IdentityRoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserClaim_UserId",
                schema: "auth",
                table: "IdentityUserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserLogin_UserId",
                schema: "auth",
                table: "IdentityUserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserRole_RoleId",
                schema: "auth",
                table: "IdentityUserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_AlbumId",
                schema: "app",
                table: "Tracks",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_PhysicalPath",
                schema: "app",
                table: "Tracks",
                column: "PhysicalPath",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudioFiles",
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
                name: "Tracks",
                schema: "app");

            migrationBuilder.DropTable(
                name: "IdentityRole",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "AppUsers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Albums",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Artists",
                schema: "app");
        }
    }
}
