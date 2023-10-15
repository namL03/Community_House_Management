using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Community_House_Management.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfficialAccounts_Persons_PersonId",
                table: "OfficialAccounts");

            migrationBuilder.DropColumn(
                name: "IsOfficial",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "CitizenId",
                table: "OfficialAccounts");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "OfficialAccounts",
                newName: "OfficialId");

            migrationBuilder.RenameIndex(
                name: "IX_OfficialAccounts_PersonId",
                table: "OfficialAccounts",
                newName: "IX_OfficialAccounts_OfficialId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_OfficialAccounts_Persons_OfficialId",
                table: "OfficialAccounts",
                column: "OfficialId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfficialAccounts_Persons_OfficialId",
                table: "OfficialAccounts");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "OfficialId",
                table: "OfficialAccounts",
                newName: "PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_OfficialAccounts_OfficialId",
                table: "OfficialAccounts",
                newName: "IX_OfficialAccounts_PersonId");

            migrationBuilder.AddColumn<bool>(
                name: "IsOfficial",
                table: "Persons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CitizenId",
                table: "OfficialAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_OfficialAccounts_Persons_PersonId",
                table: "OfficialAccounts",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
