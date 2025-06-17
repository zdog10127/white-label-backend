using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace WhiteLabel.Domain.Entity
{
    public class Address
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("street")]
        [BsonRepresentation(BsonType.String)]
        public string Street { get; set; }

        [BsonElement("neighborhood")]
        [BsonRepresentation(BsonType.String)]
        public string Neighborhood { get; set; }

        [BsonElement("number")]
        [BsonRepresentation(BsonType.Int32)]
        public int Number { get; set; }

        [BsonElement("complement")]
        [BsonRepresentation(BsonType.String)]
        public string? Complement { get; set; }

        [BsonElement("zipCode")]
        [BsonRepresentation(BsonType.Int32)]
        public int ZipCode { get; set; }

        [BsonElement("city")]
        [BsonRepresentation(BsonType.String)]
        public string City { get; set; }

        [BsonElement("state")]
        [BsonRepresentation(BsonType.String)]
        public string State { get; set; }

        [BsonElement("country")]
        [BsonRepresentation(BsonType.String)]
        public string Country { get; set; }

        [BsonElement("idClient")]
        [BsonRepresentation(BsonType.String)]
        public string IdClient { get; set; }
    }
}