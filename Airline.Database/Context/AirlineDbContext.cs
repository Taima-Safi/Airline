using Airline.Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Airline.Database.Context;

public class AirlineDbContext : DbContext
{
    public AirlineDbContext(DbContextOptions<AirlineDbContext> options) : base(options)
    {
    }

    public DbSet<UserModel> User { get; set; }
    public DbSet<CityModel> City { get; set; }
    public DbSet<SeatModel> Seat { get; set; }
    public DbSet<FlightModel> Flight { get; set; }
    public DbSet<CountryModel> Country { get; set; }
    public DbSet<AirportModel> Airport { get; set; }
    public DbSet<AirplaneModel> Airplane { get; set; }
    public DbSet<FlightClassPriceModel> FlightClassPrice { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder builder) // for relations
    {
        base.OnModelCreating(builder);
        // FlightModel -> AirportModel (Arrival)
        builder.Entity<FlightModel>()
            .HasOne(f => f.ArrivalAirport)
            .WithMany(a => a.ArrivalFlights)
            .HasForeignKey(f => f.ArrivalAirportId)
            .OnDelete(DeleteBehavior.Restrict);

        // FlightModel -> AirportModel (Departure)
        builder.Entity<FlightModel>()
            .HasOne(f => f.DepartureAirport)
            .WithMany(a => a.DepartureFlights)
            .HasForeignKey(f => f.DepartureAirportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BookModel>()
            .HasIndex(b => new { b.FlightId, b.SeatId })
            .IsUnique();
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        BeforeSaving();
        return base.SaveChangesAsync(cancellationToken);
    }
    protected void BeforeSaving()
    {
        IEnumerable<EntityEntry> entityEntries = ChangeTracker.Entries();
        DateTime utcNow = DateTime.UtcNow;
        foreach (var entityEntry in entityEntries)
        {
            if (entityEntry.Entity is BaseModel entity)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Modified:
                        entity.UpdateDate = utcNow;
                        break;
                    case EntityState.Added:
                        entity.CreateDate = utcNow;
                        break;
                }
            }
        }
    }
}
