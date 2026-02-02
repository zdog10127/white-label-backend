using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WhiteLabel.Domain.Entity
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // ==========================================
        // DADOS DO PACIENTE
        // ==========================================

        [BsonElement("patientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; }

        [BsonElement("patientName")]
        public string PatientName { get; set; }

        // ==========================================
        // DADOS DO PROFISSIONAL
        // ==========================================

        [BsonElement("professionalId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProfessionalId { get; set; }

        [BsonElement("professionalName")]
        public string ProfessionalName { get; set; }

        // ==========================================
        // DATA E HORA
        // ==========================================

        [BsonElement("date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        [BsonElement("startTime")]
        public string StartTime { get; set; } // HH:mm (ex: "14:00")

        [BsonElement("endTime")]
        public string EndTime { get; set; } // HH:mm (ex: "15:00")

        [BsonElement("duration")]
        public int Duration { get; set; } = 60; // Duração em minutos

        // ==========================================
        // TIPO DE ATENDIMENTO
        // ==========================================

        [BsonElement("type")]
        public string Type { get; set; } = "Consulta";
        // Tipos: Consulta, Retorno, Avaliação, Sessão, Emergência, Outros

        [BsonElement("specialty")]
        public string Specialty { get; set; }
        // Especialidade: Nutrição, Psicologia, Fisioterapia, Assistência Social, etc

        // ==========================================
        // STATUS
        // ==========================================

        [BsonElement("status")]
        public string Status { get; set; } = "Agendado";
        // Status: Agendado, Confirmado, EmAtendimento, Concluído, Cancelado, Faltou

        // ==========================================
        // OBSERVAÇÕES
        // ==========================================

        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("cancellationReason")]
        public string CancellationReason { get; set; }

        // ==========================================
        // LEMBRETE
        // ==========================================

        [BsonElement("reminderSent")]
        public bool ReminderSent { get; set; } = false;

        [BsonElement("reminderDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ReminderDate { get; set; }

        // ==========================================
        // METADADOS
        // ==========================================

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [BsonElement("createdBy")]
        public string CreatedBy { get; set; }

        [BsonElement("updatedBy")]
        public string UpdatedBy { get; set; }
    }
}