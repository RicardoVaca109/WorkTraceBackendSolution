namespace WorkTrace.Data.Models;

public class ClientSignature
{
    public MediaFile Signature { get; set; }
    public string SignedBy { get; set; }
    public DateTime SignedAt { get; set; }
}