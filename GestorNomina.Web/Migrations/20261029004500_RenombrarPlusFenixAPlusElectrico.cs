using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarPlusFenixAPlusElectrico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "aplica_plus_fenix",
                table: "empleados",
                newName: "aplica_plus_electrico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "aplica_plus_electrico",
                table: "empleados",
                newName: "aplica_plus_fenix");
        }
    }
}
