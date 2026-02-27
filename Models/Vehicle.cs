namespace LeafletBE.Models;

public class Vehicle
{
    public long     Id          { get; set; }           // surrogate PK (bigserial)
    public string   VehicleId   { get; set; } = "";     // natural key, unique — matches gps_tracks.vehicle_id
    public string   Name        { get; set; } = "";
    public string?  Type        { get; set; }           // e.g. truck, yard-tractor, forklift
    public string?  Description { get; set; }
    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;
}
