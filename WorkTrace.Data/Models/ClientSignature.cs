namespace WorkTrace.Data.Models;

public class ClientSignature
{
    public string SignatureBase64 { get; set; }
    public string SignedBy { get; set; }
    public DateTime SignedAt { get; set; }
}