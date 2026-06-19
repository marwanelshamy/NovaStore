using NovaStore.Services.Interfaces;

namespace NovaStore.Services.Implementations
{
    public class SmtpEmailService : IEmailService
    {
        public Task SendAsync(string to, string subject, string htmlBody) =>
            Task.CompletedTask;
    }
}