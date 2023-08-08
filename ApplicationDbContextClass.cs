using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;

namespace TareasMVC
{
    public class ApplicationDbContextClass : DbContext
    {
        public ApplicationDbContextClass(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Tarea>().Property(t => t.Titulo)
            //    .HasMaxLength(250)
            //    .IsRequired();
        }

        public DbSet<Tarea> Tareas { get; set; }
    }
}
