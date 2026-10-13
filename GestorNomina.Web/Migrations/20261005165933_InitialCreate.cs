using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestorNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "actividades",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    es_hora_efectiva = table.Column<bool>(type: "boolean", nullable: false),
                    es_licencia = table.Column<bool>(type: "boolean", nullable: false),
                    es_franco = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividades", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feriados",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    jurisdiccion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feriados", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regimenes_laborales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dias_trabajo = table.Column<int>(type: "integer", nullable: false),
                    dias_descanso = table.Column<int>(type: "integer", nullable: false),
                    max_horas_diarias = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    horas_semanales_limite = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regimenes_laborales", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reglas_normativas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    nivel_severidad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reglas_normativas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "empleados",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    legajo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    documento_identidad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    regimen_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empleados", x => x.id);
                    table.ForeignKey(
                        name: "FK_empleados_regimenes_laborales_regimen_id",
                        column: x => x.regimen_id,
                        principalTable: "regimenes_laborales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "proyectos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ubicacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    regimen_default_id = table.Column<int>(type: "integer", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proyectos", x => x.id);
                    table.ForeignKey(
                        name: "FK_proyectos_regimenes_laborales_regimen_default_id",
                        column: x => x.regimen_default_id,
                        principalTable: "regimenes_laborales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rol_id = table.Column<int>(type: "integer", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "empleado_proyecto",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empleado_id = table.Column<int>(type: "integer", nullable: false),
                    proyecto_id = table.Column<int>(type: "integer", nullable: false),
                    fecha_desde = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_hasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    regimen_especifico_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empleado_proyecto", x => x.id);
                    table.ForeignKey(
                        name: "FK_empleado_proyecto_empleados_empleado_id",
                        column: x => x.empleado_id,
                        principalTable: "empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_empleado_proyecto_proyectos_proyecto_id",
                        column: x => x.proyecto_id,
                        principalTable: "proyectos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_empleado_proyecto_regimenes_laborales_regimen_especifico_id",
                        column: x => x.regimen_especifico_id,
                        principalTable: "regimenes_laborales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "periodos_carga",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    proyecto_id = table.Column<int>(type: "integer", nullable: false),
                    mes = table.Column<int>(type: "integer", nullable: false),
                    anio = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    usuario_carga_id = table.Column<int>(type: "integer", nullable: true),
                    nombre_archivo_original = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    hash_archivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periodos_carga", x => x.id);
                    table.ForeignKey(
                        name: "FK_periodos_carga_proyectos_proyecto_id",
                        column: x => x.proyecto_id,
                        principalTable: "proyectos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_periodos_carga_usuarios_usuario_carga_id",
                        column: x => x.usuario_carga_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "horas_registros_raw",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    periodo_carga_id = table.Column<int>(type: "integer", nullable: false),
                    empleado_legajo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    empleado_id = table.Column<int>(type: "integer", nullable: true),
                    actividad_codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    actividad_id = table.Column<int>(type: "integer", nullable: true),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    horas_cargadas = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    fila_excel_origen = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horas_registros_raw", x => x.id);
                    table.ForeignKey(
                        name: "FK_horas_registros_raw_actividades_actividad_id",
                        column: x => x.actividad_id,
                        principalTable: "actividades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_horas_registros_raw_empleados_empleado_id",
                        column: x => x.empleado_id,
                        principalTable: "empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_horas_registros_raw_periodos_carga_periodo_carga_id",
                        column: x => x.periodo_carga_id,
                        principalTable: "periodos_carga",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "horas_registros_aprobados",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    registro_raw_id = table.Column<int>(type: "integer", nullable: false),
                    periodo_carga_id = table.Column<int>(type: "integer", nullable: false),
                    empleado_id = table.Column<int>(type: "integer", nullable: false),
                    actividad_id = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    horas_finales = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    fue_modificado = table.Column<bool>(type: "boolean", nullable: false),
                    motivo_modificacion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    usuario_modificador_id = table.Column<int>(type: "integer", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horas_registros_aprobados", x => x.id);
                    table.ForeignKey(
                        name: "FK_horas_registros_aprobados_actividades_actividad_id",
                        column: x => x.actividad_id,
                        principalTable: "actividades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_horas_registros_aprobados_empleados_empleado_id",
                        column: x => x.empleado_id,
                        principalTable: "empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_horas_registros_aprobados_horas_registros_raw_registro_raw_~",
                        column: x => x.registro_raw_id,
                        principalTable: "horas_registros_raw",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_horas_registros_aprobados_periodos_carga_periodo_carga_id",
                        column: x => x.periodo_carga_id,
                        principalTable: "periodos_carga",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_horas_registros_aprobados_usuarios_usuario_modificador_id",
                        column: x => x.usuario_modificador_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "validaciones_registro",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    registro_raw_id = table.Column<int>(type: "integer", nullable: false),
                    regla_id = table.Column<int>(type: "integer", nullable: false),
                    mensaje_observacion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    propuesta_sugerida = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    valor_sugerido_horas = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    actividad_sugerida_id = table.Column<int>(type: "integer", nullable: true),
                    estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validaciones_registro", x => x.id);
                    table.ForeignKey(
                        name: "FK_validaciones_registro_actividades_actividad_sugerida_id",
                        column: x => x.actividad_sugerida_id,
                        principalTable: "actividades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_validaciones_registro_horas_registros_raw_registro_raw_id",
                        column: x => x.registro_raw_id,
                        principalTable: "horas_registros_raw",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validaciones_registro_reglas_normativas_regla_id",
                        column: x => x.regla_id,
                        principalTable: "reglas_normativas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actividades_codigo",
                table: "actividades",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empleado_proyecto_empleado_id",
                table: "empleado_proyecto",
                column: "empleado_id");

            migrationBuilder.CreateIndex(
                name: "IX_empleado_proyecto_proyecto_id",
                table: "empleado_proyecto",
                column: "proyecto_id");

            migrationBuilder.CreateIndex(
                name: "IX_empleado_proyecto_regimen_especifico_id",
                table: "empleado_proyecto",
                column: "regimen_especifico_id");

            migrationBuilder.CreateIndex(
                name: "IX_empleados_legajo",
                table: "empleados",
                column: "legajo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empleados_regimen_id",
                table: "empleados",
                column: "regimen_id");

            migrationBuilder.CreateIndex(
                name: "IX_feriados_fecha_jurisdiccion",
                table: "feriados",
                columns: new[] { "fecha", "jurisdiccion" });

            migrationBuilder.CreateIndex(
                name: "IX_horas_registros_aprobados_actividad_id",
                table: "horas_registros_aprobados",
                column: "actividad_id");

            migrationBuilder.CreateIndex(
                name: "IX_horas_registros_aprobados_empleado_id",
                table: "horas_registros_aprobados",
                column: "empleado_id");

            migrationBuilder.CreateIndex(
                name: "IX_horas_registros_aprobados_periodo_carga_id_empleado_id_fecha",
                table: "horas_registros_aprobados",
                columns: new[] { "periodo_carga_id", "empleado_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_horas_registros_aprobados_registro_raw_id",
                table: "horas_registros_aprobados",
                column: "registro_raw_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_horas_registros_aprobados_usuario_modificador_id",
                table: "horas_registros_aprobados",
                column: "usuario_modificador_id");

            migrationBuilder.CreateIndex(
                name: "IX_horas_registros_raw_actividad_id",
                table: "horas_registros_raw",
                column: "actividad_id");

            migrationBuilder.CreateIndex(
                name: "IX_horas_registros_raw_empleado_id",
                table: "horas_registros_raw",
                column: "empleado_id");

            migrationBuilder.CreateIndex(
                name: "IX_horas_registros_raw_periodo_carga_id_empleado_id_fecha",
                table: "horas_registros_raw",
                columns: new[] { "periodo_carga_id", "empleado_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_periodos_carga_proyecto_id_mes_anio",
                table: "periodos_carga",
                columns: new[] { "proyecto_id", "mes", "anio" });

            migrationBuilder.CreateIndex(
                name: "IX_periodos_carga_usuario_carga_id",
                table: "periodos_carga",
                column: "usuario_carga_id");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_codigo",
                table: "proyectos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_regimen_default_id",
                table: "proyectos",
                column: "regimen_default_id");

            migrationBuilder.CreateIndex(
                name: "IX_reglas_normativas_codigo",
                table: "reglas_normativas",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_rol_id",
                table: "usuarios",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_validaciones_registro_actividad_sugerida_id",
                table: "validaciones_registro",
                column: "actividad_sugerida_id");

            migrationBuilder.CreateIndex(
                name: "IX_validaciones_registro_registro_raw_id",
                table: "validaciones_registro",
                column: "registro_raw_id");

            migrationBuilder.CreateIndex(
                name: "IX_validaciones_registro_regla_id",
                table: "validaciones_registro",
                column: "regla_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "empleado_proyecto");

            migrationBuilder.DropTable(
                name: "feriados");

            migrationBuilder.DropTable(
                name: "horas_registros_aprobados");

            migrationBuilder.DropTable(
                name: "validaciones_registro");

            migrationBuilder.DropTable(
                name: "horas_registros_raw");

            migrationBuilder.DropTable(
                name: "reglas_normativas");

            migrationBuilder.DropTable(
                name: "actividades");

            migrationBuilder.DropTable(
                name: "empleados");

            migrationBuilder.DropTable(
                name: "periodos_carga");

            migrationBuilder.DropTable(
                name: "proyectos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "regimenes_laborales");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
