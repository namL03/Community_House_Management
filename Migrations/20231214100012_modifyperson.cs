using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Community_House_Management.Migrations
{
    /// <inheritdoc />
    public partial class modifyperson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HouseholdOwnedId",
                table: "Persons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HouseholdOwnedId",
                table: "Persons",
                type: "int",
                nullable: true);
        }
    }
}
