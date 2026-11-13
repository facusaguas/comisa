using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposEdicionRecibos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "editado_por_usuario_id",
                table: "recibos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_ultima_edicion",
                table: "recibos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modo_edicion",
                table: "recibos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "observaciones_edicion",
                table: "recibos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "editado_manualmente",
                table: "recibo_detalles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "valor_original",
                table: "recibo_detalles",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "hs_100_max",
                table: "convenios",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "hs_50_max",
                table: "convenios",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "hs_normales_max",
                table: "convenios",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "es_editable",
                table: "conceptos_liquidacion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "es_obligatorio",
                table: "conceptos_liquidacion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_recibos_editado_por_usuario_id",
                table: "recibos",
                column: "editado_por_usuario_id");

            migrationBuilder.AddForeignKey(
                name: "FK_recibos_usuarios_editado_por_usuario_id",
                table: "recibos",
                column: "editado_por_usuario_id",
                principalTable: "usuarios",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recibos_usuarios_editado_por_usuario_id",
                table: "recibos");

            migrationBuilder.DropIndex(
                name: "IX_recibos_editado_por_usuario_id",
                table: "recibos");

            migrationBuilder.DropColumn(
                name: "editado_por_usuario_id",
                table: "recibos");

            migrationBuilder.DropColumn(
                name: "fecha_ultima_edicion",
                table: "recibos");

            migrationBuilder.DropColumn(
                name: "modo_edicion",
                table: "recibos");

            migrationBuilder.DropColumn(
                name: "observaciones_edicion",
                table: "recibos");

            migrationBuilder.DropColumn(
                name: "editado_manualmente",
                table: "recibo_detalles");

            migrationBuilder.DropColumn(
                name: "valor_original",
                table: "recibo_detalles");

            migrationBuilder.DropColumn(
                name: "hs_100_max",
                table: "convenios");

            migrationBuilder.DropColumn(
                name: "hs_50_max",
                table: "convenios");

            migrationBuilder.DropColumn(
                name: "hs_normales_max",
                table: "convenios");

            migrationBuilder.DropColumn(
                name: "es_editable",
                table: "conceptos_liquidacion");

            migrationBuilder.DropColumn(
                name: "es_obligatorio",
                table: "conceptos_liquidacion");
        }
    }
}
