using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestorNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaNovedades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "novedades",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empleado_id = table.Column<int>(type: "integer", nullable: false),
                    periodo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quincena = table.Column<int>(type: "integer", nullable: false),
                    tipo_novedad = table.Column<int>(type: "integer", nullable: false),
                    fecha_novedad = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    monto_novedad = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_novedades", x => x.id);
                    table.ForeignKey(
                        name: "FK_novedades_empleados_empleado_id",
                        column: x => x.empleado_id,
                        principalTable: "empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_novedades_empleado_id_periodo_quincena",
                table: "novedades",
                columns: new[] { "empleado_id", "periodo", "quincena" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "novedades");
        }
    }
}
