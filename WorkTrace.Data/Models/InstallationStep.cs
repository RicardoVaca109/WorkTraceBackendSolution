using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class InstallationStep : BaseModel
{
    public int Steps { get; set; }
    public required string Description { get; set; }
}