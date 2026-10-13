using Microsoft.EntityFrameworkCore;
using GestorNomina.Web.Models.Entities;

namespace GestorNomina.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<RegimenLaboral> RegimenesLaborales { get; set; }
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<EmpleadoProyecto> EmpleadosProyectos { get; set; }
    public DbSet<Feriado> Feriados { get; set; }
    public DbSet<ReglaNormativa> ReglasNormativas { get; set; }
    public DbSet<PeriodoCarga> PeriodosCargas { get; set; }
    public DbSet<Actividad> Actividades { get; set; }
    public DbSet<HoraRegistroRaw> HorasRegistrosRaw { get; set; }
    public DbSet<HoraRegistroAprobado> HorasRegistrosAprobados { get; set; }
    public DbSet<ValidacionRegistro> ValidacionesRegistros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de precisión para decimales
        modelBuilder.Entity<RegimenLaboral>(entity =>
        {
            entity.Property(e => e.MaxHorasDiarias).HasPrecision(5, 2);
            entity.Property(e => e.HorasSemanalesLimite).HasPrecision(5, 2);
        });

        modelBuilder.Entity<HoraRegistroRaw>(entity =>
        {
            entity.Property(e => e.HorasCargadas).HasPrecision(5, 2);
        });

        modelBuilder.Entity<HoraRegistroAprobado>(entity =>
        {
            entity.Property(e => e.HorasFinales).HasPrecision(5, 2);
        });

        modelBuilder.Entity<ValidacionRegistro>(entity =>
        {
            entity.Property(e => e.ValorSugeridoHoras).HasPrecision(5, 2);
        });

        // Configuración de relaciones

        // Usuario -> Rol
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        // Proyecto -> RegimenLaboral (Default)
        modelBuilder.Entity<Proyecto>()
            .HasOne(p => p.RegimenDefault)
            .WithMany(r => r.ProyectosConRegimenDefault)
            .HasForeignKey(p => p.RegimenDefaultId)
            .OnDelete(DeleteBehavior.SetNull);

        // Empleado -> RegimenLaboral
        modelBuilder.Entity<Empleado>()
            .HasOne(e => e.Regimen)
            .WithMany(r => r.Empleados)
            .HasForeignKey(e => e.RegimenId)
            .OnDelete(DeleteBehavior.SetNull);

        // EmpleadoProyecto -> Empleado
        modelBuilder.Entity<EmpleadoProyecto>()
            .HasOne(ep => ep.Empleado)
            .WithMany(e => e.EmpleadosProyectos)
            .HasForeignKey(ep => ep.EmpleadoId)
            .OnDelete(DeleteBehavior.Cascade);

        // EmpleadoProyecto -> Proyecto
        modelBuilder.Entity<EmpleadoProyecto>()
            .HasOne(ep => ep.Proyecto)
            .WithMany(p => p.EmpleadosProyectos)
            .HasForeignKey(ep => ep.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        // EmpleadoProyecto -> RegimenLaboral (Específico)
        modelBuilder.Entity<EmpleadoProyecto>()
            .HasOne(ep => ep.RegimenEspecifico)
            .WithMany(r => r.EmpleadosProyectosConRegimenEspecifico)
            .HasForeignKey(ep => ep.RegimenEspecificoId)
            .OnDelete(DeleteBehavior.SetNull);

        // PeriodoCarga -> Proyecto
        modelBuilder.Entity<PeriodoCarga>()
            .HasOne(pc => pc.Proyecto)
            .WithMany(p => p.PeriodosCargas)
            .HasForeignKey(pc => pc.ProyectoId)
            .OnDelete(DeleteBehavior.Restrict);

        // PeriodoCarga -> Usuario
        modelBuilder.Entity<PeriodoCarga>()
            .HasOne(pc => pc.UsuarioCarga)
            .WithMany(u => u.PeriodosCargas)
            .HasForeignKey(pc => pc.UsuarioCargaId)
            .OnDelete(DeleteBehavior.SetNull);

        // HoraRegistroRaw -> PeriodoCarga
        modelBuilder.Entity<HoraRegistroRaw>()
            .HasOne(hrr => hrr.PeriodoCarga)
            .WithMany(pc => pc.HorasRegistrosRaw)
            .HasForeignKey(hrr => hrr.PeriodoCargaId)
            .OnDelete(DeleteBehavior.Cascade);

        // HoraRegistroRaw -> Empleado
        modelBuilder.Entity<HoraRegistroRaw>()
            .HasOne(hrr => hrr.Empleado)
            .WithMany(e => e.HorasRegistrosRaw)
            .HasForeignKey(hrr => hrr.EmpleadoId)
            .OnDelete(DeleteBehavior.SetNull);

        // HoraRegistroRaw -> Actividad
        modelBuilder.Entity<HoraRegistroRaw>()
            .HasOne(hrr => hrr.Actividad)
            .WithMany(a => a.HorasRegistrosRaw)
            .HasForeignKey(hrr => hrr.ActividadId)
            .OnDelete(DeleteBehavior.SetNull);

        // HoraRegistroAprobado -> HoraRegistroRaw (1:1)
        modelBuilder.Entity<HoraRegistroAprobado>()
            .HasOne(hra => hra.RegistroRaw)
            .WithOne(hrr => hrr.HoraRegistroAprobado)
            .HasForeignKey<HoraRegistroAprobado>(hra => hra.RegistroRawId)
            .OnDelete(DeleteBehavior.Cascade);

        // HoraRegistroAprobado -> PeriodoCarga
        modelBuilder.Entity<HoraRegistroAprobado>()
            .HasOne(hra => hra.PeriodoCarga)
            .WithMany(pc => pc.HorasRegistrosAprobados)
            .HasForeignKey(hra => hra.PeriodoCargaId)
            .OnDelete(DeleteBehavior.Restrict);

        // HoraRegistroAprobado -> Empleado
        modelBuilder.Entity<HoraRegistroAprobado>()
            .HasOne(hra => hra.Empleado)
            .WithMany(e => e.HorasRegistrosAprobados)
            .HasForeignKey(hra => hra.EmpleadoId)
            .OnDelete(DeleteBehavior.Restrict);

        // HoraRegistroAprobado -> Actividad
        modelBuilder.Entity<HoraRegistroAprobado>()
            .HasOne(hra => hra.Actividad)
            .WithMany(a => a.HorasRegistrosAprobados)
            .HasForeignKey(hra => hra.ActividadId)
            .OnDelete(DeleteBehavior.Restrict);

        // HoraRegistroAprobado -> Usuario (Modificador)
        modelBuilder.Entity<HoraRegistroAprobado>()
            .HasOne(hra => hra.UsuarioModificador)
            .WithMany(u => u.HorasRegistrosModificados)
            .HasForeignKey(hra => hra.UsuarioModificadorId)
            .OnDelete(DeleteBehavior.SetNull);

        // ValidacionRegistro -> HoraRegistroRaw
        modelBuilder.Entity<ValidacionRegistro>()
            .HasOne(vr => vr.RegistroRaw)
            .WithMany(hrr => hrr.ValidacionesRegistros)
            .HasForeignKey(vr => vr.RegistroRawId)
            .OnDelete(DeleteBehavior.Cascade);

        // ValidacionRegistro -> ReglaNormativa
        modelBuilder.Entity<ValidacionRegistro>()
            .HasOne(vr => vr.Regla)
            .WithMany(rn => rn.ValidacionesRegistros)
            .HasForeignKey(vr => vr.ReglaId)
            .OnDelete(DeleteBehavior.Restrict);

        // ValidacionRegistro -> Actividad (Sugerida)
        modelBuilder.Entity<ValidacionRegistro>()
            .HasOne(vr => vr.ActividadSugerida)
            .WithMany(a => a.ValidacionesRegistrosConActividadSugerida)
            .HasForeignKey(vr => vr.ActividadSugeridaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices únicos
        modelBuilder.Entity<Rol>()
            .HasIndex(r => r.Nombre)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Proyecto>()
            .HasIndex(p => p.Codigo)
            .IsUnique();

        modelBuilder.Entity<Empleado>()
            .HasIndex(e => e.Legajo)
            .IsUnique();

        modelBuilder.Entity<Actividad>()
            .HasIndex(a => a.Codigo)
            .IsUnique();

        modelBuilder.Entity<ReglaNormativa>()
            .HasIndex(rn => rn.Codigo)
            .IsUnique();

        // Índices compuestos
        modelBuilder.Entity<Feriado>()
            .HasIndex(f => new { f.Fecha, f.Jurisdiccion });

        modelBuilder.Entity<PeriodoCarga>()
            .HasIndex(pc => new { pc.ProyectoId, pc.Mes, pc.Anio });

        modelBuilder.Entity<HoraRegistroRaw>()
            .HasIndex(hrr => new { hrr.PeriodoCargaId, hrr.EmpleadoId, hrr.Fecha });

        modelBuilder.Entity<HoraRegistroAprobado>()
            .HasIndex(hra => new { hra.PeriodoCargaId, hra.EmpleadoId, hra.Fecha });
    }
}
