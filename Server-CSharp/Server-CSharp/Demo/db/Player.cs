using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBEntity
{
    [BsonIgnoreExtraElements]
    public class Player : MongoBaseEntity
    {
        public string uid;
        public int headPhoto;
        public string playerIntroduction;
    }
}

