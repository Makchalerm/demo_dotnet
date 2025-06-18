// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using MyApiProject.Models;

namespace MyApiProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
    }
}
