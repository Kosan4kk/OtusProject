using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<StudioRoom> StudioRooms { get; set; }
        public DbSet<StudioBooking> Bookings { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("Data Source=music_studio.db");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи StudioBooking -> User
            modelBuilder.Entity<StudioBooking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Настройка связи StudioBooking -> Room
            modelBuilder.Entity<StudioBooking>()
                .HasOne(b => b.Room)
                .WithMany()
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Настройка связи Participant -> Booking
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Participants)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
