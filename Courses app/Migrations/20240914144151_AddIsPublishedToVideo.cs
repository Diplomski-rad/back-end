using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses_app.Migrations
{
    public partial class AddIsPublishedToVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Video",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Video");
        }
    }
}
