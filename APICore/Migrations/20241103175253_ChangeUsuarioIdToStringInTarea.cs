using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUsuarioIdToStringInTarea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_AspNetUsers_UsuarioId1",
                table: "Tareas");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_UsuarioId1",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Tareas");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Tareas",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_UsuarioId",
                table: "Tareas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_AspNetUsers_UsuarioId",
                table: "Tareas",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_AspNetUsers_UsuarioId",
                table: "Tareas");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_UsuarioId",
                table: "Tareas");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Tareas",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId1",
                table: "Tareas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_UsuarioId1",
                table: "Tareas",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_AspNetUsers_UsuarioId1",
                table: "Tareas",
                column: "UsuarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
