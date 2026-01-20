namespace WorkTrace.Application.DTOs.KPIsDTO;

public class DashboardQuestionMetricResponse
{
    public string QuestionKey { get; set; }
    public string QuestionText { get; set; }
    public double Average { get; set; }
    public int Count { get; set; }
}