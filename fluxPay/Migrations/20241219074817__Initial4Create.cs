using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fluxPay.Migrations
{
    /// <inheritdoc />
    public partial class _Initial4Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTokenVerified",
                table: "TempUsers");

            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "TempUsers");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TempUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationToken",
                table: "TempUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationTokenExpiration",
                table: "TempUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationToken",
                table: "TempUsers");

            migrationBuilder.DropColumn(
                name: "EmailVerificationTokenExpiration",
                table: "TempUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TempUsers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<bool>(
                name: "IsTokenVerified",
                table: "TempUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "TempUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
