using Microsoft.EntityFrameworkCore;
using NordClan.BookingApp.Api.Models;

namespace NordClan.BookingApp.Api.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime });
        }
    }
}
