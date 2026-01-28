using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entities;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalReportsController : ControllerBase
    {
        private readonly IServiceMedicalReport _service;
        private readonly ILogger<MedicalReportsController> _logger;

        public MedicalReportsController(IServiceMedicalReport service, ILogger<MedicalReportsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/medicalreports/{id}
        /// Get medical report by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("GetById attempt with null or empty ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid medical report ID",
                    Detail = "Medical report ID cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Getting medical report by ID: {ReportId}", id);

            try
            {
                var report = await _service.GetByIdAsync(id);

                if (report == null)
                {
                    _logger.LogWarning("Medical report not found: {ReportId}", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "Medical report not found",
                        Detail = $"No medical report found with ID: {id}",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogInformation("Medical report found: {ReportId}", id);

                return Ok(new
                {
                    success = true,
                    data = report
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical report {ReportId}: {Message}", id, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error retrieving medical report",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// GET: api/medicalreports/patient/{patientId}
        /// Get medical report by patient ID
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByPatientId(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                _logger.LogWarning("GetByPatientId attempt with null or empty patient ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid patient ID",
                    Detail = "Patient ID cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Getting medical report for patient: {PatientId}", patientId);

            try
            {
                var report = await _service.GetByPatientIdAsync(patientId);

                if (report == null)
                {
                    _logger.LogWarning("Medical report not found for patient: {PatientId}", patientId);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "Medical report not found",
                        Detail = $"No medical report found for patient: {patientId}",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogInformation("Medical report found for patient: {PatientId}", patientId);

                return Ok(new
                {
                    success = true,
                    data = report
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical report for patient {PatientId}: {Message}", patientId, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error retrieving medical report",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// POST: api/medicalreports
        /// Create a new medical report
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] MedicalReport report)
        {
            if (report == null)
            {
                _logger.LogWarning("Create medical report attempt with null data");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Medical report data cannot be null",
                    Detail = "Medical report data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (string.IsNullOrEmpty(report.PatientId))
            {
                _logger.LogWarning("Create medical report attempt without patient ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Patient ID is required",
                    Detail = "Medical report must have a patient ID.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Creating medical report for patient: {PatientId}", report.PatientId);

            try
            {
                var created = await _service.CreateAsync(report);

                _logger.LogInformation("Medical report created successfully: {ReportId}", created.Id);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    new
                    {
                        success = true,
                        data = created,
                        message = "Medical report created successfully"
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medical report for patient {PatientId}: {Message}", report.PatientId, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Failed to create medical report",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
        }

        /// <summary>
        /// PUT: api/medicalreports/{id}
        /// Update an existing medical report
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] MedicalReport report)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Update medical report attempt with null or empty ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid medical report ID",
                    Detail = "Medical report ID cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (report == null)
            {
                _logger.LogWarning("Update medical report attempt with null data for report {ReportId}", id);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Medical report data cannot be null",
                    Detail = "Medical report data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (string.IsNullOrEmpty(report.PatientId))
            {
                _logger.LogWarning("Update medical report attempt without patient ID for report {ReportId}", id);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Patient ID is required",
                    Detail = "Medical report must have a patient ID.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Updating medical report: {ReportId}", id);

            try
            {
                var updated = await _service.UpdateAsync(id, report);

                _logger.LogInformation("Medical report updated successfully: {ReportId}", id);

                return Ok(new
                {
                    success = true,
                    data = updated,
                    message = "Medical report updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical report {ReportId}: {Message}", id, ex.Message);

                if (ex.Message.Contains("not found"))
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "Medical report not found",
                        Detail = ex.Message,
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                var badRequestDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Failed to update medical report",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(badRequestDetails);
            }
        }

        /// <summary>
        /// DELETE: api/medicalreports/{id}
        /// Delete a medical report
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Delete medical report attempt with null or empty ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid medical report ID",
                    Detail = "Medical report ID cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Deleting medical report: {ReportId}", id);

            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Medical report not found for deletion: {ReportId}", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "Medical report not found",
                        Detail = $"No medical report found with ID: {id}",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogInformation("Medical report deleted successfully: {ReportId}", id);

                return Ok(new
                {
                    success = true,
                    message = "Medical report deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical report {ReportId}: {Message}", id, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error deleting medical report",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}