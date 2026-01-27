using System.Text.Json.Serialization;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.DTOs.AssignmentDTO.Mobile;

public class StartAssignmentRequest
{
    [JsonPropertyName("checkIn")]
    public DateTime CheckIn { get; set; }

    [JsonPropertyName("currentLocation")]
    public GeoPoint CurrentLocation { get; set; }
}