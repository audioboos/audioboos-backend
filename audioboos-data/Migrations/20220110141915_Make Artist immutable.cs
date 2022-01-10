using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioBoos.Data.Migrations
{
    public partial class MakeArtistimmutable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "immutable",
                schema: "app",
                table: "artists",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "immutable",
                schema: "app",
                table: "artists");
        }
    }
}
