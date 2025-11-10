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
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Меркурий" },
                new Room { Id = 2, Name = "Венера" },
                new Room { Id = 3, Name = "Марс" },
                new Room { Id = 4, Name = "Юпитер" },
                new Room { Id = 5, Name = "Сатурн" }
            );

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime });
        }
    }
}
