using Gym_FitByte.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym_FitByte.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ====== Tablas existentes ======
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<MembresiaHistorial> MembresiasHistorial { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<VentaVisita> VentasVisitas { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Progreso> Progresos { get; set; }

        // ====== Nuevas tablas (Proveedores / Productos / Compras / Ventas) ======
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraItem> ComprasItems { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaItem> VentasItems { get; set; }

        // ====== Pre-Registros ======
        public DbSet<PreRegistro> PreRegistros { get; set; }

        // ====== Visitas de un día ======
        public DbSet<VisitaDiaria> VisitasDiarias { get; set; }
        public DbSet<VisitaHistorial> VisitasHistorial { get; set; }

        // ====== Configuración de entidades ======
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- Membresías ----
            modelBuilder.Entity<Membresia>()
                .HasIndex(m => m.CodigoCliente)
                .IsUnique();

            modelBuilder.Entity<MembresiaHistorial>()
                .HasOne(h => h.Membresia)
                .WithMany(m => m.Historial!)
                .HasForeignKey(h => h.MembresiaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Proveedores ----
            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.RFC)
                .IsUnique();

            // ---- Productos ----
            modelBuilder.Entity<Producto>()
                .HasIndex(p => new { p.ProveedorId, p.Nombre })
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Proveedor)
                .WithMany(pr => pr.Productos)
                .HasForeignKey(p => p.ProveedorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Compras ----
            modelBuilder.Entity<Compra>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Compra!)
                .HasForeignKey(i => i.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Ventas ----
            modelBuilder.Entity<Venta>()
                .HasMany(v => v.Items)
                .WithOne(i => i.Venta!)
                .HasForeignKey(i => i.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Pre-Registros ----
            modelBuilder.Entity<PreRegistro>()
                .Property(p => p.Estado)
                .HasConversion<int>(); // guarda enum como int

            // ---- Visitas de un día ----
            modelBuilder.Entity<VisitaDiaria>()
                .HasMany(v => v.Historial)
                .WithOne(h => h.Visita!)
                .HasForeignKey(h => h.VisitaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VisitaDiaria>()
                .HasIndex(v => v.FechaHoraIngreso);

            modelBuilder.Entity<VisitaDiaria>()
                .Property(v => v.Estado)
                .HasConversion<int>(); // guarda enum como int

            modelBuilder.Entity<VisitaHistorial>()
                .HasIndex(h => new { h.VisitaId, h.Fecha });
        }
    }
}
