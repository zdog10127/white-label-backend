using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IServiceAuth _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IServiceAuth authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        [Route("health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult HealthCheck()
        {
            _logger.LogInformation("Executing HealthCheck");

            StringBuilder info = new StringBuilder();
            info.AppendLine("WhiteLabel.API - Clinic Management System");
            info.AppendLine("Status = Healthy");
            info.AppendLine($"Timestamp = {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

            _logger.LogInformation("HealthCheck executed successfully");
            return Ok(info.ToString());
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Register attempt with null data");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Registration data cannot be null",
                    Detail = "Registration data cannot be empty or null.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Executing registration for email: {Email}", dto.Email);

            try
            {
                var result = await _authService.RegisterAsync(dto);

                _logger.LogInformation("User registered successfully: {Email}", dto.Email);
                return StatusCode((int)HttpStatusCode.Created, new
                {
                    success = true,
                    data = result,
                    message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}: {Message}", dto.Email, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Registration failed",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Login attempt with null data");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Login data cannot be null",
                    Detail = "Email and password are required.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Executing login for email: {Email}", dto.Email);

            try
            {
                var result = await _authService.LoginAsync(dto);

                _logger.LogInformation("Login successful for email: {Email}", dto.Email);
                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Login failed for email {Email}: {Message}", dto.Email, ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "Unauthorized",
                    Title = "Login failed",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return Unauthorized(problemDetails);
            }
        }

        [HttpPost("verify-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult VerifyToken([FromBody] VerifyTokenDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Token))
            {
                _logger.LogWarning("Token verification attempt with null token");

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "BadRequest",
                    Title = "Token cannot be null",
                    Detail = "Token is required for verification.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }

            _logger.LogInformation("Verifying token");

            try
            {
                _logger.LogInformation("Token verified successfully");
                return Ok(new
                {
                    success = true,
                    message = "Token is valid"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token verification failed: {Message}", ex.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "Unauthorized",
                    Title = "Invalid token",
                    Detail = ex.Message,
                    Instance = HttpContext.Request.Path
                };
                return Unauthorized(problemDetails);
            }
        }
    }

    public class VerifyTokenDTO
    {
        public string Token { get; set; }
    }
}
