using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class Client : BaseModel
{
    public required string FullName { get; set; }
    public required string DocumentNumber { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
}
