using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Courses_app.Migrations
{
    public partial class AddPayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PayoutId",
                table: "AuthorEarning",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Payout",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PayoutDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payout_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorEarning_PayoutId",
                table: "AuthorEarning",
                column: "PayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Payout_AuthorId",
                table: "Payout",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorEarning_Payout_PayoutId",
                table: "AuthorEarning",
                column: "PayoutId",
                principalTable: "Payout",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorEarning_Payout_PayoutId",
                table: "AuthorEarning");

            migrationBuilder.DropTable(
                name: "Payout");

            migrationBuilder.DropIndex(
                name: "IX_AuthorEarning_PayoutId",
                table: "AuthorEarning");

            migrationBuilder.DropColumn(
                name: "PayoutId",
                table: "AuthorEarning");
        }
    }
}
