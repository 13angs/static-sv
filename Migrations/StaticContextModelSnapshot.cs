﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static_sv.Models;

#nullable disable

namespace static_sv.Migrations
{
    [DbContext(typeof(StaticContext))]
    partial class StaticContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.15");

            modelBuilder.Entity("static_sv.Models.Folder", b =>
                {
                    b.Property<long>("FolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("folder_id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_date");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("modified_date");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<long?>("ParentFolderId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("parent_folder_id");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("path");

                    b.HasKey("FolderId");

                    b.HasIndex("ParentFolderId");

                    b.ToTable("folders", (string)null);
                });

            modelBuilder.Entity("static_sv.Models.Staticfile", b =>
                {
                    b.Property<long>("StaticfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("staticfile_id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("created_date");

                    b.Property<byte[]>("FileData")
                        .HasColumnType("BLOB")
                        .HasColumnName("file_data");

                    b.Property<long?>("FolderId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("folder_id");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("modified_date");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<long?>("ParentFileId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("parent_file_id");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("path");

                    b.Property<long>("Size")
                        .HasColumnType("INTEGER")
                        .HasColumnName("size");

                    b.Property<long>("Timestamp")
                        .HasColumnType("INTEGER")
                        .HasColumnName("timestamp");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("type");

                    b.HasKey("StaticfileId");

                    b.HasIndex("FolderId");

                    b.HasIndex("ParentFileId");

                    b.ToTable("staticfiles", (string)null);
                });

            modelBuilder.Entity("static_sv.Models.Folder", b =>
                {
                    b.HasOne("static_sv.Models.Folder", "ParentFolder")
                        .WithMany("SubFolders")
                        .HasForeignKey("ParentFolderId");

                    b.Navigation("ParentFolder");
                });

            modelBuilder.Entity("static_sv.Models.Staticfile", b =>
                {
                    b.HasOne("static_sv.Models.Folder", "Folder")
                        .WithMany("Staticfiles")
                        .HasForeignKey("FolderId");

                    b.HasOne("static_sv.Models.Staticfile", "ParentFile")
                        .WithMany("RelatedFiles")
                        .HasForeignKey("ParentFileId");

                    b.Navigation("Folder");

                    b.Navigation("ParentFile");
                });

            modelBuilder.Entity("static_sv.Models.Folder", b =>
                {
                    b.Navigation("Staticfiles");

                    b.Navigation("SubFolders");
                });

            modelBuilder.Entity("static_sv.Models.Staticfile", b =>
                {
                    b.Navigation("RelatedFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
