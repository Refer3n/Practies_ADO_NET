using Microsoft.EntityFrameworkCore;
using Pract4.DAL.Entities;
using System.Configuration;
using System.Data;

namespace Pract4.DAL
{
    public class StudentsContext : DbContext
    {
        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<StudentCard> StudentCards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string conString = ConfigurationManager.ConnectionStrings["Default"].ToString();

            string conString = "Data Source=DESKTOP-VI7HLAA\\SQLSERVER;Initial Catalog=Students;Integrated Security=True;Connect Timeout=30;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(conString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .HasOne(s => s.StudentCard)
                .WithOne(s => s.Student)
                .HasForeignKey<StudentCard>(sc => sc.Id);

            var student1 = new StudentCard { Id = 1, IdNumber = "A12345", DateOfIssue = DateTime.Now, Status = true };
            var student2 = new StudentCard { Id = 2, IdNumber = "B54321", DateOfIssue = DateTime.Now, Status = true };
            var student3 = new StudentCard { Id = 3, IdNumber = "C67890", DateOfIssue = DateTime.Now, Status = false };

            modelBuilder.Entity<StudentCard>().HasData(
                student1, student2, student3
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, FirstName = "John", SecondName = "Doe", Phone = "123456789", Email = "johnDoe@gmail.com"},
                new Student { Id = 2, FirstName = "Emily", SecondName = "Smith", Phone = "987654321", Email = "janeSmith@gmail.com" },
                new Student { Id = 3, FirstName = "Mary", SecondName = "Johnson", Phone = "555555555", Email = "maryJohnson@gmail.com" }
            );
        }
    }
}