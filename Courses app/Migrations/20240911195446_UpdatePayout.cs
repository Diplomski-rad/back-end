using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses_app.Migrations
{
    public partial class UpdatePayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ControlGuid",
                table: "Payout",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControlGuid",
                table: "Payout");
        }
    }
}
