using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    /// <summary>
    /// DTO para requisição de relatório de evolução do paciente
    /// </summary>
    public class PatientReportRequestDto
    {
        /// <summary>
        /// ID do paciente
        /// </summary>
        [Required(ErrorMessage = "Patient ID is required")]
        public string PatientId { get; set; }

        /// <summary>
        /// Data inicial do período (opcional)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Data final do período (opcional)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Incluir histórico médico
        /// </summary>
        public bool IncludeMedicalHistory { get; set; } = true;

        /// <summary>
        /// Incluir evoluções
        /// </summary>
        public bool IncludeEvolutions { get; set; } = true;

        /// <summary>
        /// Incluir agendamentos
        /// </summary>
        public bool IncludeAppointments { get; set; } = true;

        /// <summary>
        /// Incluir dados antropométricos
        /// </summary>
        public bool IncludeAnthropometricData { get; set; } = true;

        /// <summary>
        /// Observações/comentários sobre o relatório
        /// </summary>
        public string? Comments { get; set; }
    }

    /// <summary>
    /// DTO para requisição de relatório consolidado
    /// </summary>
    public class ConsolidatedReportRequestDto
    {
        /// <summary>
        /// Data inicial do período
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Data final do período
        /// </summary>
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Filtrar por profissional (ID do usuário)
        /// </summary>
        public string? ProfessionalId { get; set; }

        /// <summary>
        /// Tipo de relatório (atendimentos, novos pacientes, etc)
        /// </summary>
        public string ReportType { get; set; } = "all";
    }
}