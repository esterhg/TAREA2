using Microsoft.EntityFrameworkCore;
using TAREA2.Modelos;
using TAREA2.Utilidades;


namespace TAREA2.DataAccess
{
    public class PDbContext : DbContext
    {
        public DbSet<Personas> Personas { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conexionDB = $"Filename={ConexionDB.DevolverRuta("Personas.db")}";
            optionsBuilder.UseSqlite(conexionDB);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Personas>(entity =>
            {
                entity.HasKey(col => col.IdPersona);
                entity.Property(col => col.IdPersona).IsRequired().ValueGeneratedOnAdd();
            });
        }


    }
}