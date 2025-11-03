public class NotificationService : INotificationService
{
    public async Task SendOtpEmailAsync(string email, string otp)
    {
        // Replace with SMTP or SendGrid logic
        Console.WriteLine($"Sending OTP {otp} to {email}");
        await Task.CompletedTask;
    }
}
