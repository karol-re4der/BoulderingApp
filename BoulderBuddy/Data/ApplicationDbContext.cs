using BoulderBuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace BoulderBuddy.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Gyms> Gyms { get; set; }
        public DbSet<Grades> Grades { get; set; }
        public DbSet<GradingSystems> GradingSystems { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Ascents> Ascents { get; set; }
        public DbSet<RouteComments> RouteComments { get; set; }
    }
}
