using Common.DTOs;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobTrackerApp.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);
            var success = await _authService.RegisterAsync(dto);
            if (!success)
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", dto.Email);
                return BadRequest("Email already exists.");
            }
            _logger.LogInformation("Registration successful for email: {Email}", dto.Email);
            return Ok("Registration successful.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);
            var token = await _authService.LoginAsync(dto);
            if (token == null)
            {
                _logger.LogWarning("Login failed: Invalid credentials for email: {Email}", dto.Email);
                return Unauthorized("Invalid credentials.");
            }
            _logger.LogInformation("Login successful for email: {Email}", dto.Email);
            return Ok(new { Token = token });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);
            if (!result) return BadRequest("Invalid or expired OTP");
            return Ok("Account verified");
        }

    }
}
