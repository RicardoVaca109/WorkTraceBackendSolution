namespace WorkTrace.Application.DTOs.KPIsDTO;

public class EvaluationDashboardRequest
{
    public string UserId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}