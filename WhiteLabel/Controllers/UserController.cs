using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;
using WhiteLabel.Domain.Entity.Enum;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IServiceUser _service;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IServiceUser service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/users
        /// Lista todos os usuários
        /// </summary>
        [HttpGet]
        [RequirePermission(Permissions.ViewUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("User {UserEmail} requesting all users", userEmail);

            try
            {
                var users = await _service.GetAllUsersAsync();

                _logger.LogInformation("Retrieved {Count} users successfully", users.Count);

                return Ok(new
                {
                    success = true,
                    data = users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Failed to retrieve users",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// GET: api/users/{id}
        /// Busca usuário por ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermission(Permissions.ViewUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
                    Title = "Invalid user ID",
                    Detail = "User ID cannot be null or empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("User {UserEmail} requesting user {UserId}", userEmail, id);

            try
            {
                var user = await _service.GetUserByIdAsync(id);

                _logger.LogInformation("User {UserId} retrieved successfully", id);

                return Ok(new
                {
                    success = true,
                    data = user
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found") || ex.Message.Contains("não encontrado"))
                {
                    _logger.LogWarning("User {UserId} not found", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "User not found",
                        Detail = $"User with ID {id} was not found.",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogError(ex, "Error retrieving user {UserId}: {Message}", id, ex.Message);

                var problemDetailsError = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Failed to retrieve user",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetailsError);
            }
        }

        /// <summary>
        /// POST: api/users
        /// Cria novo usuário
        /// </summary>
        [HttpPost]
        [RequirePermission(Permissions.CreateUsers)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("User creation attempt with null data");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "User data cannot be null",
                    Detail = "User data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("User creation attempt with invalid data");
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data",
                    errors = ModelState
                });
            }

            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("User {CurrentUser} creating new user {NewUserEmail}", currentUserEmail, dto.Email);

            try
            {
                var user = await _service.CreateUserAsync(dto);

                _logger.LogInformation("User created successfully: {UserId} - {UserEmail}", user.Id, user.Email);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = user.Id },
                    new
                    {
                        success = true,
                        data = user,
                        message = "User created successfully"
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserEmail}: {Message}", dto.Email, ex.Message);

                if (ex.Message.Contains("already exists") || ex.Message.Contains("já existe"))
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Title = "User already exists",
                        Detail = ex.Message,
                        Instance = HttpContext.Request.Path
                    };
                    return BadRequest(problemDetails);
                }

                if (ex.Message.Contains("Invalid role"))
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Title = "Invalid role",
                        Detail = ex.Message,
                        Instance = HttpContext.Request.Path
                    };
                    return BadRequest(problemDetails);
                }

                var problemDetailsError = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Failed to create user",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetailsError);
            }
        }

        /// <summary>
        /// PUT: api/users/{id}
        /// Atualiza usuário existente
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermission(Permissions.EditUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequestDto dto)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Update attempt with null or empty ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid user ID",
                    Detail = "User ID cannot be null or empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (dto == null)
            {
                _logger.LogWarning("User update attempt with null data for user {UserId}", id);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "User data cannot be null",
                    Detail = "User data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("User update attempt with invalid data for user {UserId}", id);
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data",
                    errors = ModelState
                });
            }

            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("User {CurrentUser} updating user {UserId}", currentUserEmail, id);

            try
            {
                var user = await _service.UpdateUserAsync(id, dto);

                _logger.LogInformation("User {UserId} updated successfully", id);

                return Ok(new
                {
                    success = true,
                    data = user,
                    message = "User updated successfully"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found") || ex.Message.Contains("não encontrado"))
                {
                    _logger.LogWarning("User {UserId} not found for update", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "User not found",
                        Detail = $"User with ID {id} was not found.",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                if (ex.Message.Contains("already exists") || ex.Message.Contains("já existe"))
                {
                    _logger.LogWarning("Email already exists while updating user {UserId}", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Title = "Email already exists",
                        Detail = ex.Message,
                        Instance = HttpContext.Request.Path
                    };
                    return BadRequest(problemDetails);
                }

                if (ex.Message.Contains("Invalid role"))
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Title = "Invalid role",
                        Detail = ex.Message,
                        Instance = HttpContext.Request.Path
                    };
                    return BadRequest(problemDetails);
                }

                _logger.LogError(ex, "Error updating user {UserId}: {Message}", id, ex.Message);

                var problemDetailsError = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Failed to update user",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetailsError);
            }
        }

        /// <summary>
        /// DELETE: api/users/{id}
        /// Deleta usuário
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermission(Permissions.DeleteUsers)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
                    Title = "Invalid user ID",
                    Detail = "User ID cannot be null or empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("User {CurrentUser} deleting user {UserId}", currentUserEmail, id);

            try
            {
                var deleted = await _service.DeleteUserAsync(id);

                if (!deleted)
                {
                    _logger.LogWarning("User {UserId} not found for deletion", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "User not found",
                        Detail = $"User with ID {id} was not found.",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogInformation("User {UserId} deleted successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found") || ex.Message.Contains("não encontrado"))
                {
                    _logger.LogWarning("User {UserId} not found for deletion", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "User not found",
                        Detail = $"User with ID {id} was not found.",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogError(ex, "Error deleting user {UserId}: {Message}", id, ex.Message);

                var problemDetailsError = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Failed to delete user",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetailsError);
            }
        }

        /// <summary>
        /// PUT: api/users/{id}/change-password
        /// Altera senha do usuário
        /// </summary>
        [HttpPut("{id}/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequestDto dto)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Change password attempt with null or empty ID");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Invalid user ID",
                    Detail = "User ID cannot be null or empty.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (dto == null)
            {
                _logger.LogWarning("Change password attempt with null data for user {UserId}", id);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Password data cannot be null",
                    Detail = "Password data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Change password attempt with invalid data for user {UserId}", id);
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data",
                    errors = ModelState
                });
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            // Usuário só pode alterar sua própria senha (exceto admin)
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentUserId != id && userRole != UserRoles.Administrator)
            {
                _logger.LogWarning("User {CurrentUser} attempted to change password of another user {UserId}", currentUserEmail, id);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Type = "Forbidden",
                    Title = "Forbidden",
                    Detail = "You can only change your own password.",
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status403Forbidden, problemDetails);
            }

            _logger.LogInformation("User {CurrentUser} changing password for user {UserId}", currentUserEmail, id);

            try
            {
                var success = await _service.ChangePasswordAsync(id, dto);

                if (!success)
                {
                    _logger.LogWarning("Incorrect current password for user {UserId}", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Title = "Incorrect password",
                        Detail = "Current password is incorrect.",
                        Instance = HttpContext.Request.Path
                    };
                    return BadRequest(problemDetails);
                }

                _logger.LogInformation("Password changed successfully for user {UserId}", id);

                return Ok(new
                {
                    success = true,
                    message = "Password changed successfully"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not found") || ex.Message.Contains("não encontrado"))
                {
                    _logger.LogWarning("User {UserId} not found for password change", id);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Title = "User not found",
                        Detail = $"User with ID {id} was not found.",
                        Instance = HttpContext.Request.Path
                    };
                    return NotFound(problemDetails);
                }

                _logger.LogError(ex, "Error changing password for user {UserId}: {Message}", id, ex.Message);

                var problemDetailsError = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Failed to change password",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetailsError);
            }
        }

        /// <summary>
        /// GET: api/users/roles
        /// Lista todas as roles disponíveis
        /// </summary>
        [HttpGet("roles")]
        [RequirePermission(Permissions.ManageRoles)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRoles()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("User {UserEmail} requesting available roles", userEmail);

            try
            {
                var roles = await _service.GetAvailableRolesAsync();

                _logger.LogInformation("Retrieved {Count} roles successfully", roles.Count);

                return Ok(new
                {
                    success = true,
                    data = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "InternalServerError",
                    Title = "Failed to retrieve roles",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}