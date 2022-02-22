﻿// <auto-generated />
using System;
using Havit.EFCoreTests.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Havit.EFCoreTests.Entity.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220222135313_Person-Name")]
    partial class PersonName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Havit.Data.EntityFrameworkCore.Model.DataSeedVersion", b =>
                {
                    b.Property<string>("ProfileName")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Version")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProfileName")
                        .HasName("PK_DataSeed");

                    b.ToTable("__DataSeed", (string)null);
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("City")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Street")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("ZipCode")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.BusinessCase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.HasKey("Id");

                    b.ToTable("BusinessCase");

                    b
                        .HasAnnotation("Caching-AllKeysEnabled", true)
                        .HasAnnotation("Caching-EntitiesEnabled", true);
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.CheckedEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("CheckedEntity");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.Modelation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BusinessCaseId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BusinessCaseId");

                    b.ToTable("Modelation");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("BossId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BossId");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.PropertyWithProtectedMembers", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("ProtectedSetterValue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PropertyWithProtectedMembers");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.CheckedEntity", b =>
                {
                    b.HasOne("Havit.EFCoreTests.Model.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Address");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.Modelation", b =>
                {
                    b.HasOne("Havit.EFCoreTests.Model.BusinessCase", "BusinessCase")
                        .WithMany("Modelations")
                        .HasForeignKey("BusinessCaseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BusinessCase");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.Person", b =>
                {
                    b.HasOne("Havit.EFCoreTests.Model.Person", "Boss")
                        .WithMany("Subordinates")
                        .HasForeignKey("BossId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Boss");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.BusinessCase", b =>
                {
                    b.Navigation("Modelations");
                });

            modelBuilder.Entity("Havit.EFCoreTests.Model.Person", b =>
                {
                    b.Navigation("Subordinates");
                });
#pragma warning restore 612, 618
        }
    }
}
