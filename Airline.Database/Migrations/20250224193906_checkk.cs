using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline.Database.Migrations
{
    /// <inheritdoc />
    public partial class checkk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SeatModelId",
                table: "Book",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Book_SeatModelId",
                table: "Book",
                column: "SeatModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Seat_SeatModelId",
                table: "Book",
                column: "SeatModelId",
                principalTable: "Seat",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Seat_SeatModelId",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Book_SeatModelId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "SeatModelId",
                table: "Book");
        }
    }
}
