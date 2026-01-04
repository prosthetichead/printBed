using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;
using PrintBed.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
        public DbSet<PrintTag> PrintTag { get; set; }
        public DbSet<Tag> Tag { get; set; }

        public DatabaseContext(IConfiguration configuration)
        {

            Configuration = configuration;

            ////IF the db already exists check that it can handle Migrations (some dbs could exist without the __EFMigrationsHistory table
            //if (Database.GetService<IRelationalDatabaseCreator>().Exists())
            //{
            //    Database.ExecuteSqlRaw(@"
            //        CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
            //            ""MigrationId"" TEXT NOT NULL CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY,
            //            ""ProductVersion"" TEXT NOT NULL
            //        );

            //        INSERT OR IGNORE INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
            //        VALUES ('20240302135834_Init', '8.0.1');        
            //    ");
            //}
            //Database.Migrate(); //run any migrations
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
                new FileType { Id = "200", Name = "Slicer Project", Extensions = "lys,chitubox", ImagePath = "/img/slicer.png", PreviewType="No Preview"},
                new FileType { Id = "300", Name = "Printer Code", Extensions = "gcode,goo", ImagePath = "/img/print.png", PreviewType = "No Preview" },
                new FileType { Id = "400", Name = "Image", Extensions = "jpg,png,webp", ImagePath = "/img/image.png", PreviewType = "Image" },
                new FileType { Id = "500", Name = "Document", Extensions = "pdf,docx,txt,md", ImagePath = "/img/doc.png", PreviewType = "No Preview" }
            );
            modelBuilder.Entity<Creator>().HasData(
                new Creator { Id = "0", Name = "Unknown", ImagePath = "/img/unknown-creator.png" },
                new Creator { Id = "100", Name = "Epic Miniatures", ImagePath = "/img/epic-miniatures.png" },
                new Creator { Id = "200", Name = "Loot Studios", ImagePath = "/img/loot-studios.png" },
                new Creator { Id = "300", Name = "Titan Forge", ImagePath = "/img/titan-forge.png" }
            );
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = "0", Name = "Uncategorised", ImagePath = "/img/uncategorised.png" },
                new Category { Id = "100", Name = "Miniatures", ImagePath = "/img/miniature.png" },
                new Category { Id = "200", Name = "Statues", ImagePath = "/img/statue.png" }
            );
        }

       

        

    }
}
