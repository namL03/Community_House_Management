﻿// <auto-generated />
using System;
using Community_House_Management.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Community_House_Management.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Community_House_Management.ModelsDb.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<DateTime>("timeEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("timeStart")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.EventProperty", b =>
                {
                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("PropertyId")
                        .HasColumnType("int");

                    b.HasKey("EventId", "PropertyId");

                    b.HasIndex("PropertyId");

                    b.ToTable("EventProperty");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Household", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Households");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.OfficialAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OfficialId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OfficialId")
                        .IsUnique();

                    b.ToTable("OfficialAccounts");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CitizenId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("HouseholdId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("HouseholdId");

                    b.ToTable("Persons");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Person");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Property", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Properties");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Official", b =>
                {
                    b.HasBaseType("Community_House_Management.ModelsDb.Person");

                    b.HasDiscriminator().HasValue("Official");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Event", b =>
                {
                    b.HasOne("Community_House_Management.ModelsDb.Person", "Person")
                        .WithMany("Events")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.EventProperty", b =>
                {
                    b.HasOne("Community_House_Management.ModelsDb.Event", "Event")
                        .WithMany("EventProperties")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Community_House_Management.ModelsDb.Property", "Property")
                        .WithMany("EventProperties")
                        .HasForeignKey("PropertyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Property");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.OfficialAccount", b =>
                {
                    b.HasOne("Community_House_Management.ModelsDb.Official", "Official")
                        .WithOne("OfficialAccount")
                        .HasForeignKey("Community_House_Management.ModelsDb.OfficialAccount", "OfficialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Official");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Person", b =>
                {
                    b.HasOne("Community_House_Management.ModelsDb.Household", "Household")
                        .WithMany("Members")
                        .HasForeignKey("HouseholdId");

                    b.Navigation("Household");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Event", b =>
                {
                    b.Navigation("EventProperties");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Household", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Person", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Property", b =>
                {
                    b.Navigation("EventProperties");
                });

            modelBuilder.Entity("Community_House_Management.ModelsDb.Official", b =>
                {
                    b.Navigation("OfficialAccount");
                });
#pragma warning restore 612, 618
        }
    }
}
