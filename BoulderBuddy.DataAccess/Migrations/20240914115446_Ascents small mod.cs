using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoulderBuddy.Migrations
{
    /// <inheritdoc />
    public partial class Ascentssmallmod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "Ascents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Ascents");
        }
    }
}
