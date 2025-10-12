using MongoDB.Bson;

namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

public class ServiceInformationResponse
{
    public string ServiceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ObjectId> InstallationSteps { get; set; }
}