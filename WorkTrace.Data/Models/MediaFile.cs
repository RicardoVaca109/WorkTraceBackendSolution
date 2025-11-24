using MongoDB.Bson;
using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class MediaFile
{
    public string Url { get; set; }
    public DateTime UploadedAt { get; set; }
}