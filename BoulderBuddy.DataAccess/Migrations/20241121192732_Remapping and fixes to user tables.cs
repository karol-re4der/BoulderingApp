using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoulderBuddy.Migrations
{
    /// <inheritdoc />
    public partial class Remappingandfixestousertables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RouteComments",
                newName: "UserDataId");

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUserId",
                table: "UserData",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserDataId",
                table: "RouteComments",
                newName: "UserId");

            migrationBuilder.AlterColumn<int>(
                name: "AspNetUserId",
                table: "UserData",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
