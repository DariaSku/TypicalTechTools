using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;


namespace TypicalTechTools.Models.Data
{
    public class TypicalTechToolsDBContext : DbContext
    {
        CsvParser csvParser;
        public TypicalTechToolsDBContext(DbContextOptions options, CsvParser csvParser) : base(options)
        {
            this.csvParser = csvParser;
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Parse products and comments from CSV
            var products = csvParser.ParseProducts();
            var comments = csvParser.ParseComments();

            int session = 1;
            // Добавим недостающие поля к комментариям:
            foreach (var comment in comments)
            {
                comment.CreatedDate = DateTime.Now; // или другое фиксированное значение
                comment.SessionId = $"session-{session++:000}"; // уникальный идентификатор, можно любой текст
            }

            foreach (var product in products)
            {
                product.UpdatedDate = DateTime.Now; // ← Здесь добавляется UpdatedDate
            }

            // Seed data to the database
            builder.Entity<Product>().HasData(products.ToList());
            builder.Entity<Comment>().HasData(comments.ToList());

            // Seed admin user
            builder.Entity<AppUser>().HasData(new AppUser
            {
                Id = 1,
                UserName = "admin",
                Role = "ADMIN",
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_1")
            });
        }
    }
}



