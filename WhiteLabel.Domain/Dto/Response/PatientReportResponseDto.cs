using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Response
{
    /// <summary>
    /// DTO de resposta com relatório completo do paciente
    /// </summary>
    public class PatientReportResponseDto
    {
        /// <summary>
        /// Informações básicas do paciente
        /// </summary>
        public PatientBasicInfo Patient { get; set; }

        /// <summary>
        /// Histórico médico
        /// </summary>
        public MedicalHistorySummary? MedicalHistory { get; set; }

        /// <summary>
        /// Lista de evoluções no período
        /// </summary>
        public List<EvolutionSummary> Evolutions { get; set; } = new();

        /// <summary>
        /// Lista de agendamentos no período
        /// </summary>
        public List<AppointmentSummary> Appointments { get; set; } = new();

        /// <summary>
        /// Dados antropométricos ao longo do tempo
        /// </summary>
        public List<AnthropometricDataPoint> AnthropometricData { get; set; } = new();

        /// <summary>
        /// Estatísticas gerais
        /// </summary>
        public ReportStatistics Statistics { get; set; }

        /// <summary>
        /// Observações/comentários sobre o relatório
        /// </summary>
        public string? Comments { get; set; }

        /// <summary>
        /// Data de geração do relatório
        /// </summary>
        public DateTime GeneratedAt { get; set; }

        /// <summary>
        /// Período do relatório
        /// </summary>
        public ReportPeriod Period { get; set; }
    }

    public class PatientBasicInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class MedicalHistorySummary
    {
        public string MainComplaint { get; set; }
        public string CurrentIllnessHistory { get; set; }
        public List<string> ChronicDiseases { get; set; } = new();
        public List<string> Medications { get; set; } = new();
        public List<string> Allergies { get; set; } = new();
        public string FamilyHistory { get; set; }
    }

    public class EvolutionSummary
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string ProfessionalName { get; set; }
        public string ProfessionalRole { get; set; }
        public string SubjectiveData { get; set; }
        public string ObjectiveData { get; set; }
        public string Assessment { get; set; }
        public string Plan { get; set; }
        public string Notes { get; set; }
    }

    public class AppointmentSummary
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string ProfessionalName { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public class AnthropometricDataPoint
    {
        public DateTime Date { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public double? BMI { get; set; }
        public double? WaistCircumference { get; set; }
        public double? HipCircumference { get; set; }
        public double? BodyFatPercentage { get; set; }
    }

    public class ReportStatistics
    {
        public int TotalEvolutions { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int MissedAppointments { get; set; }
        public double? WeightChange { get; set; }
        public double? BMIChange { get; set; }
        public int DaysInTreatment { get; set; }
    }

    public class ReportPeriod
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalDays { get; set; }
    }

    /// <summary>
    /// DTO de resposta com relatório consolidado
    /// </summary>
    public class ConsolidatedReportResponseDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalPatients { get; set; }
        public int NewPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalEvolutions { get; set; }
        public Dictionary<string, int> AppointmentsByProfessional { get; set; } = new();
        public Dictionary<string, int> AppointmentsByType { get; set; } = new();
        public Dictionary<string, int> PatientsByGender { get; set; } = new();
        public Dictionary<string, int> PatientsByAgeGroup { get; set; } = new();
        public List<TopDiagnosis> TopDiagnoses { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    public class TopDiagnosis
    {
        public string Diagnosis { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}