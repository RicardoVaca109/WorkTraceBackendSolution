namespace WorkTrace.Application.DTOs.AssignmentDTO.Mobile;

public class AssignmentMobileDashboardResponse
{
    public string Id { get; set; }
    public string Client { get; set; }
    public string Service { get; set; }
    public string Status { get; set; }
    public string Address { get; set; }
    public DateTime AssignedDate { get; set; }
    public string CreatedByUser { get; set; }
    public DateTime? CheckIn { get; set; }
}