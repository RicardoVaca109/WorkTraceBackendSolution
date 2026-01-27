namespace WorkTrace.Application.DTOs.ClientSessionDTO;

public class ClientEvaluationLoginRequest
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string DocumentNumber { get; set; }
}