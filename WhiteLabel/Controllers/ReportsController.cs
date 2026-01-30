using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhiteLabel.Domain.Dto;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Entity.Enum;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IServiceReport _service;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IServiceReport service, ILogger<ReportsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// POST: api/reports/patient
        /// Gera relatório completo de evolução do paciente
        /// </summary>
        [HttpPost("patient")]
        [RequirePermission(Permissions.ViewReports)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GeneratePatientReport([FromBody] PatientReportRequestDto request)
        {
            _logger.LogInformation("Generating patient report for patient: {PatientId}", request.PatientId);

            try
            {
                var report = await _service.GeneratePatientReportAsync(request);

                _logger.LogInformation("Patient report generated successfully for: {PatientId}", request.PatientId);

                return Ok(new
                {
                    success = true,
                    data = report,
                    message = "Report generated successfully"
                });
            }
            catch (Exception ex) when (ex.Message == "Patient not found")
            {
                _logger.LogWarning("Patient not found: {PatientId}", request.PatientId);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "NotFound",
                    Title = "Patient not found",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return NotFound(problemDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating patient report for {PatientId}: {Message}",
                    request.PatientId, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error generating report",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// POST: api/reports/consolidated
        /// Gera relatório consolidado de atendimentos
        /// </summary>
        [HttpPost("consolidated")]
        [RequirePermission(Permissions.ViewReports)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateConsolidatedReport([FromBody] ConsolidatedReportRequestDto request)
        {
            _logger.LogInformation("Generating consolidated report from {Start} to {End}",
                request.StartDate, request.EndDate);

            try
            {
                var report = await _service.GenerateConsolidatedReportAsync(request);

                _logger.LogInformation("Consolidated report generated successfully");

                return Ok(new
                {
                    success = true,
                    data = report,
                    message = "Consolidated report generated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating consolidated report: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error generating consolidated report",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}