using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WhiteLabel.Domain.Entity
{
    public class Evolution
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("patientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; } = string.Empty;

        [BsonElement("patientName")]
        public string? PatientName { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("time")]
        public string? Time { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = "Consulta";

        [BsonElement("chiefComplaint")]
        public string? ChiefComplaint { get; set; }

        [BsonElement("symptoms")]
        public string? Symptoms { get; set; }

        [BsonElement("vitalSigns")]
        public VitalSigns? VitalSigns { get; set; }

        [BsonElement("physicalExam")]
        public string? PhysicalExam { get; set; }

        [BsonElement("assessment")]
        public string? Assessment { get; set; }

        [BsonElement("conduct")]
        public string? Conduct { get; set; }

        [BsonElement("prescriptions")]
        public string? Prescriptions { get; set; }

        [BsonElement("examsRequested")]
        public string? ExamsRequested { get; set; }

        [BsonElement("treatmentEvolution")]
        public string? TreatmentEvolution { get; set; }

        [BsonElement("sideEffects")]
        public string? SideEffects { get; set; }

        [BsonElement("adherence")]
        public string? Adherence { get; set; }

        [BsonElement("notes")]
        public string? Notes { get; set; }

        [BsonElement("nextAppointment")]
        public DateTime? NextAppointment { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("attendedBy")]
        public string? AttendedBy { get; set; }

        [BsonElement("duration")]
        public int? Duration { get; set; }
    }

    public class VitalSigns
    {
        [BsonElement("bloodPressure")]
        public string? BloodPressure { get; set; }

        [BsonElement("heartRate")]
        public int? HeartRate { get; set; }

        [BsonElement("temperature")]
        public double? Temperature { get; set; }

        [BsonElement("weight")]
        public double? Weight { get; set; }

        [BsonElement("height")]
        public double? Height { get; set; }

        [BsonElement("oxygenSaturation")]
        public int? OxygenSaturation { get; set; }
    }
}