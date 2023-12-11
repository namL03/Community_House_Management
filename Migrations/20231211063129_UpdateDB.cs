using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Community_House_Management.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfficialAccounts_Persons_OfficialId",
                table: "OfficialAccounts");

            migrationBuilder.DropIndex(
                name: "IX_OfficialAccounts_OfficialId",
                table: "OfficialAccounts");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "OfficialId",
                table: "OfficialAccounts");

            migrationBuilder.AddColumn<int>(
                name: "HouseholdOwnedId",
                table: "Persons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "state",
                table: "Persons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeaderId",
                table: "Households",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Households_HeaderId",
                table: "Households",
                column: "HeaderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Households_Persons_HeaderId",
                table: "Households",
                column: "HeaderId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Households_Persons_HeaderId",
                table: "Households");

            migrationBuilder.DropIndex(
                name: "IX_Households_HeaderId",
                table: "Households");

            migrationBuilder.DropColumn(
                name: "HouseholdOwnedId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "state",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "HeaderId",
                table: "Households");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OfficialId",
                table: "OfficialAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OfficialAccounts_OfficialId",
                table: "OfficialAccounts",
                column: "OfficialId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OfficialAccounts_Persons_OfficialId",
                table: "OfficialAccounts",
                column: "OfficialId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
