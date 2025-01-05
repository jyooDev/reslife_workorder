using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReslifeWorkorderManagement.Models;
using System.Drawing;

namespace ReslifeWorkorderManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<WorkOrder> WorkOrder { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            //convert numeral enum value to string value
            modelBuilder.Entity<WorkOrder>()
                .Property(w => w.Area)
                .HasConversion(
                a => a.ToString(),
                a => (Area)Enum.Parse(typeof(Area), a));

            modelBuilder.Entity<WorkOrder>()
                .Property(w => w.Building)
                .HasConversion(
                a => a.ToString(),
                a => (Building)Enum.Parse(typeof(Building), a));

            modelBuilder.Entity<WorkOrder>()
                .Property(w => w.Progress)
                .HasConversion(
                a => a.ToString(),
                a => (Progress)Enum.Parse(typeof(Progress), a));

            //
        }
    }
}
