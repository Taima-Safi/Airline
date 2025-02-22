using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline.Database.Migrations
{
    /// <inheritdoc />
    public partial class fixFlightModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Flight",
                newName: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Flight",
                newName: "Title");
        }
    }
}
