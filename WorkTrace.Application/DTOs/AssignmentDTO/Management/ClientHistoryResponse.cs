namespace WorkTrace.Application.DTOs.AssignmentDTO.Management;

public class ClientHistoryResponse
{
    public string Service { get; set; }
    public DateTime AssignedDate { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string Status { get; set; }
    public string Address { get; set; }
    public List<string> Users { get; set; }
}