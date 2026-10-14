using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestorNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class EliminarParteDiarioRegistros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parte_diario_registros");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parte_diario_registros",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empleado_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_novedad_id = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    horas_diurnas = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    horas_nocturnas = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    periodo = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    quincena = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parte_diario_registros", x => x.id);
                    table.ForeignKey(
                        name: "FK_parte_diario_registros_empleados_empleado_id",
                        column: x => x.empleado_id,
                        principalTable: "empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parte_diario_registros_tipos_novedad_tipo_novedad_id",
                        column: x => x.tipo_novedad_id,
                        principalTable: "tipos_novedad",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_parte_diario_registros_empleado_id_fecha",
                table: "parte_diario_registros",
                columns: new[] { "empleado_id", "fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parte_diario_registros_empleado_id_periodo_quincena",
                table: "parte_diario_registros",
                columns: new[] { "empleado_id", "periodo", "quincena" });

            migrationBuilder.CreateIndex(
                name: "IX_parte_diario_registros_tipo_novedad_id",
                table: "parte_diario_registros",
                column: "tipo_novedad_id");
        }
    }
}
