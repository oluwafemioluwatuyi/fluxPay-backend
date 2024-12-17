using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fluxPay.Migrations
{
    /// <inheritdoc />
    public partial class _Initial2Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "TempUsers",
                newName: "VerificationToken");

            migrationBuilder.AddColumn<string>(
                name: "DateOfBirth",
                table: "TempUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "TempUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "TempUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IsTokenVerified",
                table: "TempUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "TempUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Passport",
                table: "TempUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "TempUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "TempUsers");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "TempUsers");

            migrationBuilder.DropColumn(
                name: "IsTokenVerified",
                table: "TempUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "TempUsers");

            migrationBuilder.DropColumn(
                name: "Passport",
                table: "TempUsers");

            migrationBuilder.RenameColumn(
                name: "VerificationToken",
                table: "TempUsers",
                newName: "FullName");
        }
    }
}
