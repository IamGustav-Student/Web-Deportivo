using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDeportivo.Migrations
{
    /// <inheritdoc />
    public partial class Token : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsPasswordResetToken",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UsPasswordResetTokenExpires",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsPasswordResetToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsPasswordResetTokenExpires",
                table: "Usuarios");
        }
    }
}
