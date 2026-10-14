using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestorNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloLiquidacionConProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "aplica_plus_altura",
                table: "empleados",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "aplica_plus_fenix",
                table: "empleados",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "categoria_id",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "convenio_id",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cuil",
                table: "empleados",
                type: "character varying(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "disciplina_id",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_alta",
                table: "empleados",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "proyecto_id",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tipo_liquidacion_id",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "valor_convenido_diario",
                table: "empleados",
                type: "numeric(12,4)",
                precision: 12,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "valor_hora_basico",
                table: "empleados",
                type: "numeric(12,4)",
                precision: 12,
                scale: 4,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "conceptos_liquidacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo_tango = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conceptos_liquidacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "convenios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_convenios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "disciplinas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disciplinas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recibos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empleado_id = table.Column<int>(type: "integer", nullable: false),
                    periodo = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    quincena = table.Column<int>(type: "integer", nullable: false),
                    dias_trabajados = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    total_bruto = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    total_deducciones = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    total_neto = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    neto_convenido_objetivo = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    diferencia_control = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: true),
                    neto_redondeado = table.Column<int>(type: "integer", nullable: false),
                    fecha_calculo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recibos", x => x.id);
                    table.ForeignKey(
                        name: "FK_recibos_empleados_empleado_id",
                        column: x => x.empleado_id,
                        principalTable: "empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tipos_liquidacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_liquidacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipos_novedad",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    requiere_horas = table.Column<bool>(type: "boolean", nullable: false),
                    color_hex_default = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_novedad", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    convenio_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id);
                    table.ForeignKey(
                        name: "FK_categorias_convenios_convenio_id",
                        column: x => x.convenio_id,
                        principalTable: "convenios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "recibo_detalles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recibo_id = table.Column<int>(type: "integer", nullable: false),
                    concepto_id = table.Column<int>(type: "integer", nullable: false),
                    unidades = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: true),
                    subtotal = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recibo_detalles", x => x.id);
                    table.ForeignKey(
                        name: "FK_recibo_detalles_conceptos_liquidacion_concepto_id",
                        column: x => x.concepto_id,
                        principalTable: "conceptos_liquidacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recibo_detalles_recibos_recibo_id",
                        column: x => x.recibo_id,
                        principalTable: "recibos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parte_diario_registros",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empleado_id = table.Column<int>(type: "integer", nullable: false),
                    periodo = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    quincena = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    horas_diurnas = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    horas_nocturnas = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    tipo_novedad_id = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_empleados_categoria_id",
                table: "empleados",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_convenio_id",
                table: "empleados",
                column: "convenio_id");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_disciplina_id",
                table: "empleados",
                column: "disciplina_id");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_proyecto_id",
                table: "empleados",
                column: "proyecto_id");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_tipo_liquidacion_id",
                table: "empleados",
                column: "tipo_liquidacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_categorias_codigo",
                table: "categorias",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categorias_convenio_id",
                table: "categorias",
                column: "convenio_id");

            migrationBuilder.CreateIndex(
                name: "IX_conceptos_liquidacion_codigo_tango",
                table: "conceptos_liquidacion",
                column: "codigo_tango",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_convenios_codigo",
                table: "convenios",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_disciplinas_codigo",
                table: "disciplinas",
                column: "codigo",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_recibo_detalles_concepto_id",
                table: "recibo_detalles",
                column: "concepto_id");

            migrationBuilder.CreateIndex(
                name: "IX_recibo_detalles_recibo_id",
                table: "recibo_detalles",
                column: "recibo_id");

            migrationBuilder.CreateIndex(
                name: "IX_recibos_empleado_id_periodo_quincena",
                table: "recibos",
                columns: new[] { "empleado_id", "periodo", "quincena" });

            migrationBuilder.CreateIndex(
                name: "IX_tipos_liquidacion_codigo",
                table: "tipos_liquidacion",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tipos_novedad_codigo",
                table: "tipos_novedad",
                column: "codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_categorias_categoria_id",
                table: "empleados",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_convenios_convenio_id",
                table: "empleados",
                column: "convenio_id",
                principalTable: "convenios",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_disciplinas_disciplina_id",
                table: "empleados",
                column: "disciplina_id",
                principalTable: "disciplinas",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_proyectos_proyecto_id",
                table: "empleados",
                column: "proyecto_id",
                principalTable: "proyectos",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_tipos_liquidacion_tipo_liquidacion_id",
                table: "empleados",
                column: "tipo_liquidacion_id",
                principalTable: "tipos_liquidacion",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_empleados_categorias_categoria_id",
                table: "empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_empleados_convenios_convenio_id",
                table: "empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_empleados_disciplinas_disciplina_id",
                table: "empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_empleados_proyectos_proyecto_id",
                table: "empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_empleados_tipos_liquidacion_tipo_liquidacion_id",
                table: "empleados");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "disciplinas");

            migrationBuilder.DropTable(
                name: "parte_diario_registros");

            migrationBuilder.DropTable(
                name: "recibo_detalles");

            migrationBuilder.DropTable(
                name: "tipos_liquidacion");

            migrationBuilder.DropTable(
                name: "convenios");

            migrationBuilder.DropTable(
                name: "tipos_novedad");

            migrationBuilder.DropTable(
                name: "conceptos_liquidacion");

            migrationBuilder.DropTable(
                name: "recibos");

            migrationBuilder.DropIndex(
                name: "IX_empleados_categoria_id",
                table: "empleados");

            migrationBuilder.DropIndex(
                name: "IX_empleados_convenio_id",
                table: "empleados");

            migrationBuilder.DropIndex(
                name: "IX_empleados_disciplina_id",
                table: "empleados");

            migrationBuilder.DropIndex(
                name: "IX_empleados_proyecto_id",
                table: "empleados");

            migrationBuilder.DropIndex(
                name: "IX_empleados_tipo_liquidacion_id",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "aplica_plus_altura",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "aplica_plus_fenix",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "categoria_id",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "convenio_id",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "cuil",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "disciplina_id",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "fecha_alta",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "proyecto_id",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "tipo_liquidacion_id",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "valor_convenido_diario",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "valor_hora_basico",
                table: "empleados");
        }
    }
}
