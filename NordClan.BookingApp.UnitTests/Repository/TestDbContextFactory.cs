using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NordClan.BookingApp.Api.Data;

namespace NordClan.BookingApp.UnitTests.Repository
{
    public class TestDbContextFactory
    {
        public static BookingDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestBookingDb_{Guid.NewGuid()}")
                .ConfigureWarnings(warnings =>
                {
                    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                })
                .EnableSensitiveDataLogging()
                .Options;

            return new BookingDbContext(options);
        }
    }
}
