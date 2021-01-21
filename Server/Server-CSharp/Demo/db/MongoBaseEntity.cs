using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
namespace DBEntity
{
    public class MongoBaseEntity
    {
        [BsonIgnore]
        ObjectId Id { get; set; }
        string Creator { get; set; }
        string CreateDate { get; set; }
        string LastEditer { get; set; }
        string LastEditDate { get; set; }
        string SystemName { get; }
        string EX { get; set; }
        string IP { get; }
    }
}