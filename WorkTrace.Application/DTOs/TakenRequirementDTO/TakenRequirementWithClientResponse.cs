using WorkTrace.Application.DTOs.ClientDTO.Information;

namespace WorkTrace.Application.DTOs.TakenRequirementDTO;

public class TakenRequirementWithClientResponse
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public ClientInformationResponse? Client { get; set; }
}