using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class CreateAppointmentRequestDto
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public string PatientId { get; set; }

        [Required(ErrorMessage = "Patient name is required")]
        public string PatientName { get; set; }

        [Required(ErrorMessage = "Professional ID is required")]
        public string ProfessionalId { get; set; }

        [Required(ErrorMessage = "Professional name is required")]
        public string ProfessionalName { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public string EndTime { get; set; }

        public int Duration { get; set; } = 60;

        public string Type { get; set; } = "Consulta";

        public string Specialty { get; set; }

        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO para atualizar agendamento
    /// </summary>
    public class UpdateAppointmentDto
    {
        public DateTime? Date { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int? Duration { get; set; }

        public string Type { get; set; }

        public string Specialty { get; set; }

        public string Status { get; set; }

        public string Notes { get; set; }
    }

    /// <summary>
    /// DTO para cancelar agendamento
    /// </summary>
    public class CancelAppointmentDto
    {
        [Required(ErrorMessage = "Cancellation reason is required")]
        public string CancellationReason { get; set; }
    }

    /// <summary>
    /// DTO para buscar agendamentos
    /// </summary>
    public class AppointmentQueryDto
    {
        public string PatientId { get; set; }
        public string ProfessionalId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }
}