﻿using MongoDB.Bson;
using WorkTrace.Data.Common.Generics;

namespace WorkTrace.Data.Models;

public class Assignment : BaseModel
{
    public required List<ObjectId> Users { get; set; }
    public ObjectId Service { get; set; }
    public ObjectId Client { get; set; }
    public ObjectId Status { get; set; }
    public DateTime Date { get; set; }
    public required string Address { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }

}
