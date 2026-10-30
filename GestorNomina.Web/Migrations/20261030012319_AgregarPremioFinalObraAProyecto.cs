using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPremioFinalObraAProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "aplica_plus_fenix",
                table: "empleados",
                newName: "aplica_plus_electrico");

            migrationBuilder.AddColumn<decimal>(
                name: "premio_final_obra",
                table: "proyectos",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "premio_final_obra",
                table: "proyectos");

            migrationBuilder.RenameColumn(
                name: "aplica_plus_electrico",
                table: "empleados",
                newName: "aplica_plus_fenix");
        }
    }
}
