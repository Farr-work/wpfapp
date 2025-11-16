using Microsoft.EntityFrameworkCore;
using StudentManager.Models;

namespace StudentManager.Models
{
    public class StudentContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }     // THÊM

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=students.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().HasKey(s => s.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique(); // username duy nhất
        }
    }
}
