using MongoDB.Bson;
using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class MediaFile :  BaseModel
{
    public ObjectId Assignment {  get; set; }
    public ObjectId User {  get; set; }
    public string FileName { get; set; }
    public string Url { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
}