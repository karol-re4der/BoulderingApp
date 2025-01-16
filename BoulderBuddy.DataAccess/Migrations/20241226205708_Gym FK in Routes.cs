using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoulderBuddy.Migrations
{
    /// <inheritdoc />
    public partial class GymFKinRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GymId",
                table: "Routes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GymId",
                table: "Routes");
        }
    }
}
