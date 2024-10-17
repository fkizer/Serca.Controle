
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Serca.Controle.Core.Domain.Entities;

namespace Serca.Controle.Core.Application.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<DeviceParametersEntity> DeviceParameters { get; set; }
        public DbSet<UtilisateurEntity> Utilisateurs { get; set; }

        public DbSet<Trace> Traces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DefineDeviceParametersEntityTable(modelBuilder);
            DefineUtilisateurEntityTable(modelBuilder);
        }

        private static void DefineDeviceParametersEntityTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceParametersEntity>()
                .HasKey(p => p.UtilisateurId);

            // Disabling autoincrement
            modelBuilder.Entity<DeviceParametersEntity>().Property(t => t.UtilisateurId).ValueGeneratedNever();

            modelBuilder.Entity<DeviceParametersEntity>().Ignore(t => t.Id);
        }

        private static void DefineUtilisateurEntityTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UtilisateurEntity>()
                .HasKey(p => p.UtilisateurId);

            // Disabling autoincrement
            modelBuilder.Entity<UtilisateurEntity>().Property(t => t.UtilisateurId).ValueGeneratedNever();

            modelBuilder.Entity<UtilisateurEntity>().Ignore(t => t.Id);
        }
    }

    
    /// <summary>
    /// Used for EFCore Tools
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite("Data Source=app.sqlite3");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

}
