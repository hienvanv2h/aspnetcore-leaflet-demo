using LeafletBE.Data;
using LeafletBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeafletBE.Controllers;

[ApiController]
[Route("api/gps-tracks")]
public class GpsTracksController(AppDbContext db) : ControllerBase
{
    /// <summary>Latest position of every vehicle (DISTINCT ON for TimescaleDB efficiency)</summary>
    [HttpGet("vehicles")]
    public async Task<IActionResult> GetVehiclesPosition() =>
        Ok(await db.GpsTracks
            .FromSqlRaw(
                "SELECT DISTINCT ON (vehicle_id) * FROM gps_tracks ORDER BY vehicle_id, time DESC")
            .ToListAsync());

    /// <summary>Track history for one vehicle within a time window (default: last 1 hour)</summary>
    [HttpGet("{vehicleId}")]
    public async Task<IActionResult> GetTrackHistory(
        string vehicleId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var fromUtc = (from?.ToUniversalTime()) ?? DateTime.UtcNow.AddHours(-1);
        var toUtc   = (to?.ToUniversalTime())   ?? DateTime.UtcNow;

        return Ok(await db.GpsTracks
            .Where(t => t.VehicleId == vehicleId && t.Time >= fromUtc && t.Time <= toUtc)
            .OrderBy(t => t.Time)
            .ToListAsync());
    }

    /// <summary>Delete all GPS tracks for a vehicle</summary>
    [HttpDelete("{vehicleId}")]
    public async Task<IActionResult> DeleteVehicleTracks(string vehicleId)
    {
        var deleted = await db.GpsTracks
            .Where(t => t.VehicleId == vehicleId)
            .ExecuteDeleteAsync();
        return Ok(new { deleted });
    }

    /// <summary>Ingest a single GPS point (for real device integration)</summary>
    [HttpPost]
    public async Task<IActionResult> Ingest([FromBody] GpsPointDto dto)
    {
        var track = new GpsTrack
        {
            Time      = dto.Time?.ToUniversalTime() ?? DateTime.UtcNow,
            VehicleId = dto.VehicleId,
            Lat       = dto.Lat,
            Lng       = dto.Lng,
            Speed     = dto.Speed,
            Heading   = dto.Heading,
        };
        db.GpsTracks.Add(track);
        await db.SaveChangesAsync();
        return Created($"/api/gps-tracks/{track.VehicleId}", track);
    }
}

public record GpsPointDto(
    string    VehicleId,
    double    Lat,
    double    Lng,
    DateTime? Time,
    float?    Speed,
    float?    Heading);
