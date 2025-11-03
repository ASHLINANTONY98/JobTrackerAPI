public interface INotificationService
{
    Task SendOtpEmailAsync(string email, string otp);
}
