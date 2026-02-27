using LeafletBE.Data;
using LeafletBE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeafletBE.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Vehicles.OrderBy(v => v.VehicleId).ToListAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var v = await db.Vehicles.FindAsync(id);
        return v is null ? NotFound() : Ok(v);
    }

    [HttpGet("by-vehicle-id/{vehicleId}")]
    public async Task<IActionResult> GetByVehicleId(string vehicleId) =>
        Ok(await db.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == vehicleId));


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VehicleDto dto)
    {
        var v = new Vehicle
        {
            VehicleId   = dto.VehicleId,
            Name        = dto.Name,
            Type        = dto.Type,
            Description = dto.Description,
            CreatedAt   = DateTime.UtcNow,
        };
        db.Vehicles.Add(v);
        await db.SaveChangesAsync();
        return Created($"/api/vehicles/{v.Id}", v);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] VehicleDto dto)
    {
        var v = await db.Vehicles.FindAsync(id);
        if (v is null) return NotFound();
        v.VehicleId   = dto.VehicleId;
        v.Name        = dto.Name;
        v.Type        = dto.Type;
        v.Description = dto.Description;
        await db.SaveChangesAsync();
        return Ok(v);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await db.Vehicles.Where(v => v.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? Ok(new { deleted }) : NotFound();
    }
}

public record VehicleDto(
    string  VehicleId,
    string  Name,
    string? Type,
    string? Description);
