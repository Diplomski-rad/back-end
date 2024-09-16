using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses_app.Migrations
{
    public partial class AddIsValidToRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Rating",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Rating");
        }
    }
}
