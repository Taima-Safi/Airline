using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline.Database.Migrations
{
    /// <inheritdoc />
    public partial class Bookl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookModel_Flight_FlightId",
                table: "BookModel");

            migrationBuilder.DropForeignKey(
                name: "FK_BookModel_Seat_SeatId",
                table: "BookModel");

            migrationBuilder.DropForeignKey(
                name: "FK_BookModel_User_UserId",
                table: "BookModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookModel",
                table: "BookModel");

            migrationBuilder.RenameTable(
                name: "BookModel",
                newName: "Book");

            migrationBuilder.RenameIndex(
                name: "IX_BookModel_UserId",
                table: "Book",
                newName: "IX_Book_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookModel_SeatId",
                table: "Book",
                newName: "IX_Book_SeatId");

            migrationBuilder.RenameIndex(
                name: "IX_BookModel_FlightId_SeatId",
                table: "Book",
                newName: "IX_Book_FlightId_SeatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Book",
                table: "Book",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Flight_FlightId",
                table: "Book",
                column: "FlightId",
                principalTable: "Flight",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Seat_SeatId",
                table: "Book",
                column: "SeatId",
                principalTable: "Seat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Book_User_UserId",
                table: "Book",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Flight_FlightId",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_Book_Seat_SeatId",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_Book_User_UserId",
                table: "Book");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Book",
                table: "Book");

            migrationBuilder.RenameTable(
                name: "Book",
                newName: "BookModel");

            migrationBuilder.RenameIndex(
                name: "IX_Book_UserId",
                table: "BookModel",
                newName: "IX_BookModel_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Book_SeatId",
                table: "BookModel",
                newName: "IX_BookModel_SeatId");

            migrationBuilder.RenameIndex(
                name: "IX_Book_FlightId_SeatId",
                table: "BookModel",
                newName: "IX_BookModel_FlightId_SeatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookModel",
                table: "BookModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookModel_Flight_FlightId",
                table: "BookModel",
                column: "FlightId",
                principalTable: "Flight",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookModel_Seat_SeatId",
                table: "BookModel",
                column: "SeatId",
                principalTable: "Seat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookModel_User_UserId",
                table: "BookModel",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
