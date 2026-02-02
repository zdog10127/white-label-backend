using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WhiteLabel.Domain.Dto;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Entity.Enum;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IServiceAppointment _service;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IServiceAppointment service, ILogger<AppointmentsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }

        /// <summary>
        /// GET: api/appointments
        /// Listar todos os agendamentos
        /// </summary>
        [HttpGet]
        [RequirePermission(Permissions.ViewAppointments)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var appointments = await _service.GetAllAsync();
                return Ok(new { success = true, data = appointments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments");
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error getting appointments",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: api/appointments/{id}
        /// Buscar agendamento por ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermission(Permissions.ViewAppointments)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var appointment = await _service.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = 404,
                        Title = "Appointment not found",
                        Detail = $"Appointment with ID {id} not found"
                    });
                }

                return Ok(new { success = true, data = appointment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment {Id}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error getting appointment",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: api/appointments/patient/{patientId}
        /// Buscar agendamentos por paciente
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [RequirePermission(Permissions.ViewAppointments)]
        public async Task<IActionResult> GetByPatient(string patientId)
        {
            try
            {
                var appointments = await _service.GetByPatientIdAsync(patientId);
                return Ok(new { success = true, data = appointments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for patient {PatientId}", patientId);
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error getting appointments",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: api/appointments/professional/{professionalId}
        /// Buscar agendamentos por profissional
        /// </summary>
        [HttpGet("professional/{professionalId}")]
        [RequirePermission(Permissions.ViewAppointments)]
        public async Task<IActionResult> GetByProfessional(string professionalId)
        {
            try
            {
                var appointments = await _service.GetByProfessionalIdAsync(professionalId);
                return Ok(new { success = true, data = appointments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for professional {ProfessionalId}", professionalId);
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error getting appointments",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// POST: api/appointments/query
        /// Buscar agendamentos com filtros
        /// </summary>
        [HttpPost("query")]
        [RequirePermission(Permissions.ViewAppointments)]
        public async Task<IActionResult> Query([FromBody] AppointmentQueryDto query)
        {
            try
            {
                var appointments = await _service.QueryAsync(query);
                return Ok(new { success = true, data = appointments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying appointments");
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error querying appointments",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// POST: api/appointments
        /// Criar novo agendamento
        /// </summary>
        [HttpPost]
        [RequirePermission(Permissions.CreateAppointments)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDto dto)
        {
            try
            {
                var userId = GetUserId();
                var appointment = await _service.CreateAsync(dto, userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = appointment.Id },
                    new { success = true, data = appointment, message = "Appointment created successfully" }
                );
            }
            catch (Exception ex) when (ex.Message.Contains("not available"))
            {
                return Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Time slot not available",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error creating appointment",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// PUT: api/appointments/{id}
        /// Atualizar agendamento
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermission(Permissions.EditAppointments)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateAppointmentDto dto)
        {
            try
            {
                var userId = GetUserId();
                var appointment = await _service.UpdateAsync(id, dto, userId);

                return Ok(new { success = true, data = appointment, message = "Appointment updated successfully" });
            }
            catch (Exception ex) when (ex.Message == "Appointment not found")
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Appointment not found",
                    Detail = ex.Message
                });
            }
            catch (Exception ex) when (ex.Message.Contains("not available"))
            {
                return Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Time slot not available",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {Id}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error updating appointment",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// POST: api/appointments/{id}/cancel
        /// Cancelar agendamento
        /// </summary>
        [HttpPost("{id}/cancel")]
        [RequirePermission(Permissions.DeleteAppointments)]
        public async Task<IActionResult> Cancel(string id, [FromBody] CancelAppointmentDto dto)
        {
            try
            {
                var userId = GetUserId();
                await _service.CancelAsync(id, dto, userId);

                return Ok(new { success = true, message = "Appointment cancelled successfully" });
            }
            catch (Exception ex) when (ex.Message == "Appointment not found")
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Appointment not found",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment {Id}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error cancelling appointment",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// DELETE: api/appointments/{id}
        /// Deletar agendamento
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermission(Permissions.DeleteAppointments)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new { success = true, message = "Appointment deleted successfully" });
            }
            catch (Exception ex) when (ex.Message == "Appointment not found")
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Appointment not found",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {Id}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error deleting appointment",
                    Detail = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: api/appointments/check-availability
        /// Verificar disponibilidade de horário
        /// </summary>
        [HttpGet("check-availability")]
        [RequirePermission(Permissions.ViewAppointments)]
        public async Task<IActionResult> CheckAvailability(
            [FromQuery] string professionalId,
            [FromQuery] DateTime date,
            [FromQuery] string startTime,
            [FromQuery] string endTime)
        {
            try
            {
                var isAvailable = await _service.CheckAvailabilityAsync(professionalId, date, startTime, endTime);
                return Ok(new { success = true, available = isAvailable });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability");
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Error checking availability",
                    Detail = ex.Message
                });
            }
        }
    }
}