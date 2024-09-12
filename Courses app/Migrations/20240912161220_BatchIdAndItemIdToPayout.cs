using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses_app.Migrations
{
    public partial class BatchIdAndItemIdToPayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Payout_batch_id",
                table: "Payout",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payout_item_id",
                table: "Payout",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payout_batch_id",
                table: "Payout");

            migrationBuilder.DropColumn(
                name: "Payout_item_id",
                table: "Payout");
        }
    }
}
