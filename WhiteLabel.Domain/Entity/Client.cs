using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace WhiteLabel.Domain.Entity
{
    public class Client
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [BsonElement("socialName")]
        [BsonRepresentation(BsonType.String)]
        public string? SocialName { get; set; }

        [BsonElement("naturalness")]
        [BsonRepresentation(BsonType.String)]
        public string Naturalness { get; set; }

        [BsonElement("dtBirth")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DtBirth { get; set; }

        [BsonElement("age")]
        [BsonRepresentation(BsonType.Int32)]
        public int Age { get; set; }

        [BsonElement("gender")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Gender { get; set; }

        [BsonElement("email")]
        [BsonRepresentation(BsonType.String)]
        public string Email { get; set; }

        [BsonElement("cellPhone")]
        [BsonRepresentation(BsonType.String)]
        public string CellPhone { get; set; }

        [BsonElement("cpf")]
        [BsonRepresentation(BsonType.String)]
        public string CPF { get; set; }

        [BsonElement("rg")]
        [BsonRepresentation(BsonType.String)]
        public string RG { get; set; }

        [BsonElement("comments")]
        [BsonRepresentation(BsonType.String)]
        public string? Comments { get; set; }
    }
}