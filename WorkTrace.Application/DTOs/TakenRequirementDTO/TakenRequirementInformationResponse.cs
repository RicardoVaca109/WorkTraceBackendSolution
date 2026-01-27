namespace WorkTrace.Application.DTOs.TakenRequirementDTO;

public class TakenRequirementInformationResponse
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string? ClientId { get; set; }
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}