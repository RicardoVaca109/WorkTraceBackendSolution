using System.Text.Json.Serialization;

namespace WorkTrace.Data.Models;

public class GeoPoint
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}