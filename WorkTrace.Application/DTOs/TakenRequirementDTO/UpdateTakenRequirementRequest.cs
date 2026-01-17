namespace WorkTrace.Application.DTOs.TakenRequirementDTO;

public class UpdateTakenRequirementRequest
{
    public string? ClientId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}