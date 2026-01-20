namespace WorkTrace.Application.DTOs.ClientSessionDTO;

public class ClientEvaluationLoginResponse
{
    public string SessionToken { get; set; }
    public string AssignmentId { get; set; }
    public DateTime ExpiresAt { get; set; }
}