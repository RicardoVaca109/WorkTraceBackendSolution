namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

public class UpdateServiceRequest
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}
