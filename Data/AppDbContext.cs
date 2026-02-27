using LeafletBE.Models;
using Microsoft.EntityFrameworkCore;

namespace LeafletBE.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<GpsTrack> GpsTracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GpsTrack>(entity =>
        {
            entity.ToTable("gps_tracks");
            entity.HasKey(e => new { e.Time, e.VehicleId });
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.Lat).HasColumnName("lat");
            entity.Property(e => e.Lng).HasColumnName("lng");
            entity.Property(e => e.Speed).HasColumnName("speed");
            entity.Property(e => e.Heading).HasColumnName("heading");
        });
    }
}
