namespace WorkTrace.Application.DTOs.ServiceMgmtDTO.Management
{
    public class ServiceInformationResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // 🔹 Cambiado de List<string> a List<InstallationStepInformationResponse>
        public List<InstallationStepInformationResponse> InstallationSteps { get; set; }
    }
}