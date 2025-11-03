using Common.DTOs;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.Interfaces;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;

        public AuthService(IUserRepository userRepo, IConfiguration config, INotificationService notificationService)
        {
            _userRepo = userRepo;
            _config = config;
            _notificationService = notificationService;
        }

        //public async Task<bool> RegisterAsync(RegisterDto dto)
        //{
        //    var existing = await _userRepo.GetByEmailAsync(dto.Email);
        //    if (existing != null) return false;

        //    var user = new User
        //    {
        //        Username = dto.Username,
        //        Email = dto.Email,
        //        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        //        Role = dto.Role // Save the role from the DTO
        //    };

        //    await _userRepo.AddAsync(user);
        //    return true;
        //}

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null) return false;

            var otp = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                OtpCode = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(5),
                IsVerified = false
            };

            await _userRepo.AddAsync(user);
            await _notificationService.SendOtpEmailAsync(user.Email, otp);

            return true;
        }


        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;
            if (!user.IsVerified)
                return null; // or throw new Exception("Account not verified");

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<bool> VerifyOtpAsync(OtpVerifyDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || user.OtpCode != dto.OtpCode || user.OtpExpiry < DateTime.UtcNow)
                return false;

            user.IsVerified = true;
            user.OtpCode = null;
            user.OtpExpiry = null;

            await _userRepo.UpdateAsync(user);
            return true;
        }
    }
}
