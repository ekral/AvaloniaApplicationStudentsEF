using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace AvaloniaApplicationStudentsEF.Models
{
    public class SchoolContext : DbContext
    {
        public DbSet<Student> Students { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = System.IO.Path.Join(directory, "students.db");
            SqliteConnectionStringBuilder connectionStringBuilder = new() { DataSource = path };
            optionsBuilder.UseSqlite(connectionStringBuilder.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student() { Id = 1, Name = "Zina" },
                new Student() { Id = 2, Name = "Vratko" },
                new Student() { Id = 3, Name = "Jitka" });
        }
    }
}
