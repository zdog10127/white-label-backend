using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Service
{
    public class ServiceReport : IServiceReport
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IRepositoryMedicalReport _medicalReportRepository;
        private readonly IRepositoryEvolution _evolutionRepository;

        public ServiceReport(
            IPatientRepository patientRepository,
            IRepositoryMedicalReport medicalReportRepository,
            IRepositoryEvolution evolutionRepository)
        {
            _patientRepository = patientRepository;
            _medicalReportRepository = medicalReportRepository;
            _evolutionRepository = evolutionRepository;
        }

        public async Task<PatientReportResponseDto> GeneratePatientReportAsync(PatientReportRequestDto request)
        {
            // Buscar paciente
            var patient = await _patientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new Exception("Patient not found");
            }

            var report = new PatientReportResponseDto
            {
                Patient = new PatientBasicInfo
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    Cpf = patient.CPF,
                    BirthDate = patient.BirthDate,
                    Age = CalculateAge(patient.BirthDate),
                    Gender = patient.Gender,
                    Phone = patient.Phone,
                    Email = patient.Email
                },
                Comments = request.Comments,
                GeneratedAt = DateTime.UtcNow,
                Period = new ReportPeriod
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalDays = request.StartDate.HasValue && request.EndDate.HasValue
                        ? (request.EndDate.Value - request.StartDate.Value).Days
                        : 0
                }
            };

            // Buscar histórico médico
            if (request.IncludeMedicalHistory)
            {
                var medicalReports = await _medicalReportRepository.FindAsync(mr => mr.PatientId == request.PatientId);
                var latestReport = medicalReports.OrderByDescending(mr => mr.CreatedAt).FirstOrDefault();

                if (latestReport != null)
                {
                    report.MedicalHistory = new MedicalHistorySummary
                    {
                        MainComplaint = latestReport.Diagnosis ?? string.Empty,
                        CurrentIllnessHistory = latestReport.GeneralNotes ?? string.Empty,
                        ChronicDiseases = !string.IsNullOrEmpty(latestReport.Comorbidities)
                            ? latestReport.Comorbidities.Split(',').Select(x => x.Trim()).ToList()
                            : new List<string>(),
                        Medications = !string.IsNullOrEmpty(latestReport.Medications)
                            ? latestReport.Medications.Split(',').Select(x => x.Trim()).ToList()
                            : new List<string>(),
                        Allergies = !string.IsNullOrEmpty(latestReport.Allergies)
                            ? latestReport.Allergies.Split(',').Select(x => x.Trim()).ToList()
                            : new List<string>(),
                        FamilyHistory = latestReport.FamilyHistory ?? string.Empty
                    };
                }
            }

            // Buscar evoluções
            if (request.IncludeEvolutions)
            {
                var evolutions = await _evolutionRepository.FindAsync(e => e.PatientId == request.PatientId);

                // Filtrar por data se necessário
                if (request.StartDate.HasValue)
                {
                    evolutions = evolutions.Where(e => e.Date >= request.StartDate.Value).ToList();
                }
                if (request.EndDate.HasValue)
                {
                    evolutions = evolutions.Where(e => e.Date <= request.EndDate.Value).ToList();
                }

                report.Evolutions = evolutions
                    .OrderByDescending(e => e.Date)
                    .Select(e => new EvolutionSummary
                    {
                        Id = e.Id,
                        Date = e.Date,
                        ProfessionalName = e.AttendedBy ?? "N/A",
                        ProfessionalRole = e.Type ?? "N/A",
                        SubjectiveData = e.ChiefComplaint ?? string.Empty,
                        ObjectiveData = e.PhysicalExam ?? string.Empty,
                        Assessment = e.Assessment ?? string.Empty,
                        Plan = e.Conduct ?? string.Empty,
                        Notes = e.Notes ?? string.Empty
                    })
                    .ToList();
            }

            // Buscar dados antropométricos
            if (request.IncludeAnthropometricData)
            {
                var evolutions = await _evolutionRepository.FindAsync(e => e.PatientId == request.PatientId);

                // Filtrar por data
                if (request.StartDate.HasValue)
                {
                    evolutions = evolutions.Where(e => e.Date >= request.StartDate.Value).ToList();
                }
                if (request.EndDate.HasValue)
                {
                    evolutions = evolutions.Where(e => e.Date <= request.EndDate.Value).ToList();
                }

                report.AnthropometricData = evolutions
                    .Where(e => e.VitalSigns != null)
                    .OrderBy(e => e.Date)
                    .Select(e => new AnthropometricDataPoint
                    {
                        Date = e.Date,
                        Weight = e.VitalSigns.Weight,
                        Height = e.VitalSigns.Height,
                        BMI = CalculateBMI(e.VitalSigns.Weight, e.VitalSigns.Height),
                        WaistCircumference = null, // Evolution não tem esses campos
                        HipCircumference = null,
                        BodyFatPercentage = null
                    })
                    .ToList();
            }

            // Calcular estatísticas
            report.Statistics = CalculateStatistics(report);

            return report;
        }

        public async Task<ConsolidatedReportResponseDto> GenerateConsolidatedReportAsync(ConsolidatedReportRequestDto request)
        {
            var report = new ConsolidatedReportResponseDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                GeneratedAt = DateTime.UtcNow
            };

            // Buscar todos os pacientes
            var allPatients = await _patientRepository.GetAllAsync();

            // Total de pacientes
            report.TotalPatients = allPatients.Count();

            // Novos pacientes no período
            var newPatients = allPatients.Where(p =>
                p.RegistrationDate >= request.StartDate &&
                p.RegistrationDate <= request.EndDate
            ).ToList();
            report.NewPatients = newPatients.Count;

            // Buscar todas as evoluções do período
            var allEvolutions = await _evolutionRepository.GetAllAsync();
            var periodEvolutions = allEvolutions.Where(e =>
                e.Date >= request.StartDate &&
                e.Date <= request.EndDate
            ).ToList();

            report.TotalEvolutions = periodEvolutions.Count;
            report.TotalAppointments = periodEvolutions.Count; // Assumindo que cada evolução = 1 atendimento

            // Distribuição por gênero
            report.PatientsByGender = allPatients
                .GroupBy(p => p.Gender ?? "Não informado")
                .ToDictionary(g => g.Key, g => g.Count());

            // Distribuição por faixa etária
            report.PatientsByAgeGroup = allPatients
                .Select(p => new { Patient = p, Age = CalculateAge(p.BirthDate) })
                .GroupBy(p => GetAgeGroup(p.Age))
                .ToDictionary(g => g.Key, g => g.Count());

            // Atendimentos por profissional
            report.AppointmentsByProfessional = periodEvolutions
                .GroupBy(e => e.AttendedBy ?? "Não informado")
                .ToDictionary(g => g.Key, g => g.Count());

            return report;
        }

        // ==========================================
        // MÉTODOS AUXILIARES
        // ==========================================

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        private double? CalculateBMI(double? weight, double? height)
        {
            if (!weight.HasValue || !height.HasValue || height.Value == 0)
                return null;

            return weight.Value / (height.Value * height.Value);
        }

        private string GetAgeGroup(int age)
        {
            if (age < 18) return "0-17 anos";
            if (age < 30) return "18-29 anos";
            if (age < 40) return "30-39 anos";
            if (age < 50) return "40-49 anos";
            if (age < 60) return "50-59 anos";
            if (age < 70) return "60-69 anos";
            return "70+ anos";
        }

        private ReportStatistics CalculateStatistics(PatientReportResponseDto report)
        {
            var stats = new ReportStatistics
            {
                TotalEvolutions = report.Evolutions.Count,
                TotalAppointments = report.Appointments.Count,
                CompletedAppointments = report.Appointments.Count(a => a.Status == "Completed"),
                CancelledAppointments = report.Appointments.Count(a => a.Status == "Cancelled"),
                MissedAppointments = report.Appointments.Count(a => a.Status == "Missed")
            };

            // Calcular mudança de peso e IMC
            if (report.AnthropometricData.Count >= 2)
            {
                var firstData = report.AnthropometricData.First();
                var lastData = report.AnthropometricData.Last();

                if (firstData.Weight.HasValue && lastData.Weight.HasValue)
                {
                    stats.WeightChange = lastData.Weight.Value - firstData.Weight.Value;
                }

                if (firstData.BMI.HasValue && lastData.BMI.HasValue)
                {
                    stats.BMIChange = lastData.BMI.Value - firstData.BMI.Value;
                }

                stats.DaysInTreatment = (lastData.Date - firstData.Date).Days;
            }

            return stats;
        }
    }
}