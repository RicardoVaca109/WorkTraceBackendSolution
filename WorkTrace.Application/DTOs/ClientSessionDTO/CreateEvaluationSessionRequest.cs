namespace WorkTrace.Application.DTOs.ClientSessionDTO;

public class CreateEvaluationSessionRequest
{
    public int ExpirationMinutes { get; set; } = 120;
}