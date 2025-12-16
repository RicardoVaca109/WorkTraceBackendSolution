using WorkTrace.Data.Models;

namespace WorkTrace.Application.DTOs.AssignmentDTO.Management;

public class AssignmentTrackingResponse
{
    public string Id { get; set; }
    public string Client { get; set; }
    public string Service { get; set; }
    public string Address { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public GeoPoint? CurrentLocation { get; set; }
    public GeoPoint? DestinationLocation { get; set; }
}