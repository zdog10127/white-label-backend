using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace WhiteLabel.Domain.Entity
{
    public class AdditionalData
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("profession")]
        [BsonRepresentation(BsonType.String)]
        public string Profession { get; set; }

        [BsonElement("kinshipExists")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool KinshipExists { get; set; }

        [BsonElement("parentName")]
        [BsonRepresentation(BsonType.String)]
        public string? ParentName { get; set; }

        [BsonElement("kinship")]
        [BsonRepresentation(BsonType.String)]
        public string? Kinship { get; set; }

        [BsonElement("cellPhone")]
        [BsonRepresentation(BsonType.String)]
        public string CellPhone { get; set; }

        [BsonElement("whereDidMeet")]
        [BsonRepresentation(BsonType.String)]
        public string? WhereDidMeet { get; set; }

        [BsonElement("forwadedBy")]
        [BsonRepresentation(BsonType.String)]
        public string ForwadedBy { get; set; }

        [BsonElement("idClient")]
        [BsonRepresentation(BsonType.String)]
        public string IdClient { get; set; }
    }
}