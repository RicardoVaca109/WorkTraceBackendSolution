

using System.Globalization;

namespace WorkTrace.Application.DTOs.AssignmentDTO.Management;

public class ClientHistoryResponse
{
    public string Service { get; set; }
    public DateTime Date { get; set; }
    public DateTime? CheckOut { get; set; }
    public string Status { get; set; }
    public string Address { get; set; }
    public List<string> Users { get; set; }
}