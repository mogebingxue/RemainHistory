using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBEntity
{
    [BsonIgnoreExtraElements]
    public class Friend : MongoBaseEntity
    {
        public string uid;
        public string friendId;
    }
}

