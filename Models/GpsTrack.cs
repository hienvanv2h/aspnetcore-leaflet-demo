namespace LeafletBE.Models;

public class GpsTrack
{
    public DateTime Time { get; set; }      // partition key (TimescaleDB)
    public string VehicleId { get; set; } = "";
    public double Lat { get; set; }
    public double Lng { get; set; }
    public float? Speed { get; set; }       // km/h
    public float? Heading { get; set; }     // degrees 0–360
}
