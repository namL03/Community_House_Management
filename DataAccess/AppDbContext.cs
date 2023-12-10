using Community_House_Management.ModelsDb;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.DataAccess
{
    class AppDbContext : DbContext
    {
        public DbSet<Property> Properties { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<OfficialAccount> OfficialAccounts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databaseFilePath = App.GetDatabaseFilePath();
            optionsBuilder.UseSqlServer($"Server=(localdb)\\mssqllocaldb;AttachDbFilename={databaseFilePath};Database=CommunityHouse;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventProperty>()
                .HasKey(ep => new { ep.EventId, ep.PropertyId });

            modelBuilder.Entity<EventProperty>()
                .HasOne(ep => ep.Event)
                .WithMany(e => e.EventProperties)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventProperty>()
                .HasOne(ep => ep.Property)
                .WithMany(p => p.EventProperties)
                .HasForeignKey(ep => ep.PropertyId);

            modelBuilder.Entity<Person>()
                .HasOne(p => p.Household)
                .WithMany(h => h.Members)
                .HasForeignKey(p => p.HouseholdId)
                .IsRequired(false);
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Person)
                .WithMany(p => p.Events)
                .HasForeignKey(e => e.PersonId);
            modelBuilder.Entity<Household>()
                .HasOne(h => h.Header) 
                .WithOne(p => p.HouseholdOwned) 
                .HasForeignKey<Household>(h => h.HeaderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
