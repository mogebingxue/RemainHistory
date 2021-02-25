
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
namespace DBEntity
{
    [BsonIgnoreExtraElements]
    public class Account : MongoBaseEntity
    {
        public string uid;
        public string pw;
    }
}


