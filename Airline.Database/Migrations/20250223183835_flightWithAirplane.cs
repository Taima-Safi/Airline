using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline.Database.Migrations
{
    /// <inheritdoc />
    public partial class flightWithAirplane : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AirplaneId",
                table: "Flight",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Flight_AirplaneId",
                table: "Flight",
                column: "AirplaneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flight_Airplane_AirplaneId",
                table: "Flight",
                column: "AirplaneId",
                principalTable: "Airplane",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flight_Airplane_AirplaneId",
                table: "Flight");

            migrationBuilder.DropIndex(
                name: "IX_Flight_AirplaneId",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "AirplaneId",
                table: "Flight");
        }
    }
}
