using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WhiteLabel.Domain.Entity
{
    public class MedicalReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("patientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; } = string.Empty;

        [BsonElement("patientName")]
        public string? PatientName { get; set; }

        [BsonElement("diagnosis")]
        public string? Diagnosis { get; set; }

        [BsonElement("medications")]
        public string? Medications { get; set; }

        [BsonElement("allergies")]
        public string? Allergies { get; set; }

        [BsonElement("comorbidities")]
        public string? Comorbidities { get; set; }

        [BsonElement("familyHistory")]
        public string? FamilyHistory { get; set; }

        [BsonElement("treatmentPlan")]
        public string? TreatmentPlan { get; set; }

        [BsonElement("recommendations")]
        public string? Recommendations { get; set; }

        [BsonElement("restrictions")]
        public string? Restrictions { get; set; }

        [BsonElement("generalNotes")]
        public string? GeneralNotes { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("createdBy")]
        public string? CreatedBy { get; set; }

        [BsonElement("updatedBy")]
        public string? UpdatedBy { get; set; }
    }
}