using MongoDB.Bson;
using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class Service : BaseModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required List<ObjectId> InstallationSteps { get; set; }
}
