using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDeportivo.Migrations
{
    /// <inheritdoc />
    public partial class Login : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoNombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UsApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UsEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UsPasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsFechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsActivo = table.Column<bool>(type: "bit", nullable: false),
                    RoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsId);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RoId",
                        column: x => x.RoId,
                        principalTable: "Roles",
                        principalColumn: "RoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RoId",
                table: "Usuarios",
                column: "RoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
