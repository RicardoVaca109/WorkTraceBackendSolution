namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;

public class ServiceInformationResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> InstallationSteps { get; set; }
}