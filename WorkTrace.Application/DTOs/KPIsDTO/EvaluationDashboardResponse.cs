namespace WorkTrace.Application.DTOs.KPIsDTO;

public class EvaluationDashboardResponse
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int TotalEvaluations { get; set; }
    public List<DashboardQuestionMetricResponse> Metrics { get; set; }
}