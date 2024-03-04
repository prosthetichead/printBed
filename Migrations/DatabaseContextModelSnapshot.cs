﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PrintBed.Models;

#nullable disable

namespace PrintBed.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("PrintBed.Models.Category", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImagePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Category");

                    b.HasData(
                        new
                        {
                            Id = "0",
                            ImagePath = "/img/uncategorised.png",
                            Name = "Uncategorised"
                        },
                        new
                        {
                            Id = "100",
                            ImagePath = "/img/miniature.png",
                            Name = "Miniatures"
                        },
                        new
                        {
                            Id = "200",
                            ImagePath = "/img/statue.png",
                            Name = "Statues"
                        });
                });

            modelBuilder.Entity("PrintBed.Models.Creator", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImagePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Creator");

                    b.HasData(
                        new
                        {
                            Id = "0",
                            ImagePath = "/img/unknown-creator.png",
                            Name = "Unknown"
                        },
                        new
                        {
                            Id = "100",
                            ImagePath = "/img/epic-miniatures.png",
                            Name = "Epic Miniatures"
                        },
                        new
                        {
                            Id = "200",
                            ImagePath = "/img/loot-studios.png",
                            Name = "Loot Studios"
                        },
                        new
                        {
                            Id = "300",
                            ImagePath = "/img/titan-forge.png",
                            Name = "Titan Forge"
                        });
                });

            modelBuilder.Entity("PrintBed.Models.FileType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Extensions")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImagePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("PreviewType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FileType");

                    b.HasData(
                        new
                        {
                            Id = "0",
                            Extensions = "",
                            ImagePath = "/img/unknown.png",
                            Name = "Unknown File Type",
                            PreviewType = "No Preview"
                        },
                        new
                        {
                            Id = "100",
                            Extensions = "stl,obj",
                            ImagePath = "/img/cube.png",
                            Name = "Model",
                            PreviewType = "Model Viewer"
                        },
                        new
                        {
                            Id = "200",
                            Extensions = "lys,chitubox",
                            ImagePath = "/img/slicer.png",
                            Name = "Slicer Project",
                            PreviewType = "No Preview"
                        },
                        new
                        {
                            Id = "300",
                            Extensions = "gcode,goo",
                            ImagePath = "/img/print.png",
                            Name = "Printer Code",
                            PreviewType = "No Preview"
                        },
                        new
                        {
                            Id = "400",
                            Extensions = "jpg,png,webp",
                            ImagePath = "/img/image.png",
                            Name = "Image",
                            PreviewType = "Image"
                        },
                        new
                        {
                            Id = "500",
                            Extensions = "pdf,docx,txt,md",
                            ImagePath = "/img/doc.png",
                            Name = "Document",
                            PreviewType = "No Preview"
                        });
                });

            modelBuilder.Entity("PrintBed.Models.Print", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PrintInstructions")
                        .HasColumnType("TEXT");

                    b.Property<string>("TagString")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CreatorId");

                    b.ToTable("Print");
                });

            modelBuilder.Entity("PrintBed.Models.PrintFile", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("FileSize")
                        .HasColumnType("REAL");

                    b.Property<string>("FileTypeId")
                        .HasColumnType("TEXT");

                    b.Property<bool?>("IsPreivew")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("PrintId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FileTypeId");

                    b.HasIndex("PrintId");

                    b.ToTable("PrintFile");
                });

            modelBuilder.Entity("PrintBed.Models.PrintTag", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("PrintId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TagId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PrintId");

                    b.HasIndex("TagId");

                    b.ToTable("PrintTag");
                });

            modelBuilder.Entity("PrintBed.Models.Tag", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("PrintBed.Models.Print", b =>
                {
                    b.HasOne("PrintBed.Models.Category", "Category")
                        .WithMany("Prints")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PrintBed.Models.Creator", "Creator")
                        .WithMany("Prints")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("PrintBed.Models.PrintFile", b =>
                {
                    b.HasOne("PrintBed.Models.FileType", "FileType")
                        .WithMany("PrintFiles")
                        .HasForeignKey("FileTypeId");

                    b.HasOne("PrintBed.Models.Print", "Print")
                        .WithMany("PrintFiles")
                        .HasForeignKey("PrintId");

                    b.Navigation("FileType");

                    b.Navigation("Print");
                });

            modelBuilder.Entity("PrintBed.Models.PrintTag", b =>
                {
                    b.HasOne("PrintBed.Models.Print", "Print")
                        .WithMany("PrintTags")
                        .HasForeignKey("PrintId");

                    b.HasOne("PrintBed.Models.Tag", "Tag")
                        .WithMany("PrintTags")
                        .HasForeignKey("TagId");

                    b.Navigation("Print");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("PrintBed.Models.Category", b =>
                {
                    b.Navigation("Prints");
                });

            modelBuilder.Entity("PrintBed.Models.Creator", b =>
                {
                    b.Navigation("Prints");
                });

            modelBuilder.Entity("PrintBed.Models.FileType", b =>
                {
                    b.Navigation("PrintFiles");
                });

            modelBuilder.Entity("PrintBed.Models.Print", b =>
                {
                    b.Navigation("PrintFiles");

                    b.Navigation("PrintTags");
                });

            modelBuilder.Entity("PrintBed.Models.Tag", b =>
                {
                    b.Navigation("PrintTags");
                });
#pragma warning restore 612, 618
        }
    }
}
