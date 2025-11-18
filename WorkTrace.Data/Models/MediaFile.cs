using MongoDB.Bson;
using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class MediaFile : BaseModel
{
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public string Url { get; set; }
    public ObjectId User {  get; set; }
}