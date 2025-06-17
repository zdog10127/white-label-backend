using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Entity
{
    public class User
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [BsonElement("email")]
        [BsonRepresentation(BsonType.String)]
        public string Email { get; set; }

        [BsonElement("cpf")]
        [BsonRepresentation(BsonType.String)]
        public string CPF { get; set; }

        [BsonElement("password")]
        [BsonRepresentation(BsonType.String)]
        public string Password { get; set; }

        [BsonElement("profile")]
        [BsonRepresentation(BsonType.String)]
        public string Profile { get; set; }
    }
}