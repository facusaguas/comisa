using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarConceptoLiquidacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_conceptos_liquidacion_codigo_tango",
                table: "conceptos_liquidacion");

            migrationBuilder.DropColumn(
                name: "codigo_tango",
                table: "conceptos_liquidacion");

            migrationBuilder.RenameColumn(
                name: "tipo",
                table: "conceptos_liquidacion",
                newName: "codigo");

            migrationBuilder.AddColumn<int>(
                name: "tipo_concepto",
                table: "conceptos_liquidacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_conceptos_liquidacion_codigo",
                table: "conceptos_liquidacion",
                column: "codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_conceptos_liquidacion_codigo",
                table: "conceptos_liquidacion");

            migrationBuilder.DropColumn(
                name: "tipo_concepto",
                table: "conceptos_liquidacion");

            migrationBuilder.RenameColumn(
                name: "codigo",
                table: "conceptos_liquidacion",
                newName: "tipo");

            migrationBuilder.AddColumn<string>(
                name: "codigo_tango",
                table: "conceptos_liquidacion",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_conceptos_liquidacion_codigo_tango",
                table: "conceptos_liquidacion",
                column: "codigo_tango",
                unique: true);
        }
    }
}
