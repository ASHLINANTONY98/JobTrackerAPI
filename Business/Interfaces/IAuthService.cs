using Common.DTOs;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
        Task<bool> VerifyOtpAsync(OtpVerifyDto dto);
    }
}
