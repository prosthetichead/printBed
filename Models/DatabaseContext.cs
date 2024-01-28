using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using PrintBed.Models;

namespace PrintBed.Models
{
    public class DatabaseContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DbSet<Print> Print { get; set; }
        public DbSet<PrintFile> PrintFile { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<FileType> FileType { get; set; }
        public DbSet<Creator> Creator { get; set; }

        public DatabaseContext(IConfiguration configuration)
        {

            Configuration = configuration;
            Database.EnsureCreated();
            //Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("Database"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileType>().HasData(
                new FileType { Id = "0", Name = "Unknown File Type", Extensions = "", ImagePath = "/img/unknown.png", PreviewType = "No Preview" },
                new FileType { Id = "100", Name = "Model", Extensions = "stl,obj", ImagePath = "/img/cube.png", PreviewType="Model Viewer" },
                new FileType { Id = "200", Name = "Slicer Project", Extensions = "lys", ImagePath = "/img/slicer.png", PreviewType="No Preview"},
                new FileType { Id = "300", Name = "Printer Code", Extensions = "gcode,goo", ImagePath = "/img/print.png", PreviewType = "No Preview" },
                new FileType { Id = "400", Name = "Image", Extensions = "jpg,png,webp", ImagePath = "/img/image.png", PreviewType = "Image" },
                new FileType { Id = "500", Name = "Document", Extensions = "pdf,docx,txt,md", ImagePath = "/img/doc.png", PreviewType = "No Preview" }
            );
            modelBuilder.Entity<Creator>().HasData(
                new Creator { Id = "0", Name = "Unknown", ImagePath = "/img/unknown-creator.png" }
            );
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = "0", Name = "Uncategorised", ImagePath = "/img/uncategorised.png" }
            );
        }

       

        

    }
}
