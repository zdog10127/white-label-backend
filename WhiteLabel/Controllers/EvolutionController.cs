using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entities;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EvolutionsController : ControllerBase
    {
        private readonly IServiceEvolution _service;
        private readonly ILogger<EvolutionsController> _logger;

        public EvolutionsController(IServiceEvolution service, ILogger<EvolutionsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/evolutions
        /// Get all evolutions
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all evolutions");

            try
            {
                var evolutions = await _service.GetAllAsync();

                _logger.LogInformation("Retrieved {Count} evolutions", evolutions.Count());

                return Ok(new
                {
                    success = true,
                    data = evolutions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all evolutions: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error retrieving evolutions",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// GET: api/evolutions/{id}
        /// Get evolution by ID
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
                    Title = "Invalid evolution ID",
                    Detail = "Evolution ID cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Getting evolution by ID: {EvolutionId}", id);

            try
            {
                var evolution = await _service.GetByIdAsync(id);

                if (evolution == null)
                {
                    _logger.LogWarning("Evolution not found: {EvolutionId}", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "Evolution not found",
                        Detail = $"No evolution found with ID: {id}",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogInformation("Evolution found: {EvolutionId}", id);

                return Ok(new
                {
                    success = true,
                    data = evolution
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting evolution {EvolutionId}: {Message}", id, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error retrieving evolution",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// GET: api/evolutions/patient/{patientId}
        /// Get all evolutions for a patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

            _logger.LogInformation("Getting evolutions for patient: {PatientId}", patientId);

            try
            {
                var evolutions = await _service.GetByPatientIdAsync(patientId);

                _logger.LogInformation("Retrieved {Count} evolutions for patient {PatientId}", evolutions.Count(), patientId);

                return Ok(new
                {
                    success = true,
                    data = evolutions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting evolutions for patient {PatientId}: {Message}", patientId, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error retrieving evolutions",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// POST: api/evolutions
        /// Create a new evolution
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] Evolution evolution)
        {
            if (evolution == null)
            {
                _logger.LogWarning("Create evolution attempt with null data");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Evolution data cannot be null",
                    Detail = "Evolution data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (string.IsNullOrEmpty(evolution.PatientId))
            {
                _logger.LogWarning("Create evolution attempt without patient ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Patient ID is required",
                    Detail = "Evolution must have a patient ID.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Creating evolution for patient: {PatientId}", evolution.PatientId);

            try
            {
                var created = await _service.CreateAsync(evolution);

                _logger.LogInformation("Evolution created successfully: {EvolutionId}", created.Id);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    new
                    {
                        success = true,
                        data = created,
                        message = "Evolution created successfully"
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating evolution for patient {PatientId}: {Message}", evolution.PatientId, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Failed to create evolution",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
        }

        /// <summary>
        /// PUT: api/evolutions/{id}
        /// Update an existing evolution
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] Evolution evolution)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Update evolution attempt with null or empty ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid evolution ID",
                    Detail = "Evolution ID cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (evolution == null)
            {
                _logger.LogWarning("Update evolution attempt with null data for evolution {EvolutionId}", id);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Evolution data cannot be null",
                    Detail = "Evolution data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (string.IsNullOrEmpty(evolution.PatientId))
            {
                _logger.LogWarning("Update evolution attempt without patient ID for evolution {EvolutionId}", id);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Patient ID is required",
                    Detail = "Evolution must have a patient ID.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Updating evolution: {EvolutionId}", id);

            try
            {
                var updated = await _service.UpdateAsync(id, evolution);

                _logger.LogInformation("Evolution updated successfully: {EvolutionId}", id);

                return Ok(new
                {
                    success = true,
                    data = updated,
                    message = "Evolution updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating evolution {EvolutionId}: {Message}", id, ex.Message);

                if (ex.Message.Contains("not found"))
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "Evolution not found",
                        Detail = ex.Message,
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                var badRequestDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Failed to update evolution",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(badRequestDetails);
            }
        }

        /// <summary>
        /// DELETE: api/evolutions/{id}
        /// Delete an evolution
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
                _logger.LogWarning("Delete evolution attempt with null or empty ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid evolution ID",
                    Detail = "Evolution ID cannot be empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Deleting evolution: {EvolutionId}", id);

            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("Evolution not found for deletion: {EvolutionId}", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "Evolution not found",
                        Detail = $"No evolution found with ID: {id}",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogInformation("Evolution deleted successfully: {EvolutionId}", id);

                return Ok(new
                {
                    success = true,
                    message = "Evolution deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting evolution {EvolutionId}: {Message}", id, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Error deleting evolution",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}