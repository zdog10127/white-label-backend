using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IServicePatient _patientService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IServicePatient patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreatePatientRequestDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Patient creation attempt with null data");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Patient data cannot be null",
                    Detail = "Patient data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            _logger.LogInformation("Creating patient {PatientName} by user {UserEmail}", dto.Name, userEmail);

            try
            {
                var result = await _patientService.CreateAsync(dto, userId);

                _logger.LogInformation("Patient created successfully: {PatientId} - {PatientName}", result.Id, result.Name);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    new
                    {
                        success = true,
                        data = result,
                        message = "Patient created successfully"
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient {PatientName}: {Message}", dto.Name, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Failed to create patient",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
                    Title = "Invalid patient ID",
                    Detail = "Patient ID cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Getting patient by ID: {PatientId}", id);

            try
            {
                var result = await _patientService.GetByIdAsync(id);

                _logger.LogInformation("Patient found: {PatientId} - {PatientName}", result.Id, result.Name);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Patient not found with ID {PatientId}: {Message}", id, ex.Message);

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
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all patients");

            try
            {
                var result = await _patientService.GetAllAsync();

                _logger.LogInformation("Retrieved {Count} patients", ((System.Collections.ICollection)result).Count);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all patients: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error retrieving patients",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePatientRequestDto dto)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Update attempt with null or empty ID");

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

            if (dto == null)
            {
                _logger.LogWarning("Update attempt with null data for patient {PatientId}", id);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Update data cannot be null",
                    Detail = "Patient update data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Updating patient {PatientId} by user {UserEmail}", id, userEmail);

            try
            {
                var result = await _patientService.UpdateAsync(id, dto);

                _logger.LogInformation("Patient updated successfully: {PatientId} - {PatientName}", result.Id, result.Name);

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Patient updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientId}: {Message}", id, ex.Message);

                var status = ex.Message.Contains("not found")
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status400BadRequest;

                var problemDetails = new ProblemDetails
                {
                    Status = status,
                    Type = status == StatusCodes.Status404NotFound ? "NotFound" : "BadRequest",
                    Title = status == StatusCodes.Status404NotFound ? "Patient not found" : "Failed to update patient",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(status, problemDetails);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Delete attempt with null or empty ID");

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

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Deleting patient {PatientId} by user {UserEmail}", id, userEmail);

            try
            {
                var result = await _patientService.DeleteAsync(id);

                _logger.LogInformation("Patient deleted successfully: {PatientId}", id);

                return Ok(new
                {
                    success = true,
                    message = "Patient deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient {PatientId}: {Message}", id, ex.Message);

                var status = ex.Message.Contains("not found")
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status400BadRequest;

                var problemDetails = new ProblemDetails
                {
                    Status = status,
                    Type = status == StatusCodes.Status404NotFound ? "NotFound" : "BadRequest",
                    Title = status == StatusCodes.Status404NotFound ? "Patient not found" : "Failed to delete patient",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(status, problemDetails);
            }
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                _logger.LogWarning("GetByStatus attempt with null or empty status");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid status",
                    Detail = "Status cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Getting patients by status: {Status}", status);

            try
            {
                var result = await _patientService.GetByStatusAsync(status);

                _logger.LogInformation("Retrieved {Count} patients with status {Status}",
                    ((System.Collections.ICollection)result).Count, status);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patients by status {Status}: {Message}", status, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error retrieving patients",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetActive()
        {
            _logger.LogInformation("Getting active patients");

            try
            {
                var result = await _patientService.GetActiveAsync();

                _logger.LogInformation("Retrieved {Count} active patients",
                    ((System.Collections.ICollection)result).Count);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active patients: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error retrieving active patients",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}