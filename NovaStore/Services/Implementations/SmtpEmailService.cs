using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using NovaStore.Settings;
using NovaStore.Services.Interfaces;

namespace NovaStore.Services.Implementations
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public SmtpEmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(
     string to,
     string subject,
     string body)
        {
            var smtp = new SmtpClient(_settings.SmtpHost)
            {
                Port = _settings.SmtpPort,
                Credentials = new NetworkCredential(
                    _settings.SenderEmail,
                    _settings.Password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(
                    _settings.SenderEmail,
                    _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await smtp.SendMailAsync(mail);
        }

        public async Task SendOrderConfirmationAsync(
            string customerEmail,
            string customerName,
            int orderId,
            decimal total)
        {
            var smtp = new SmtpClient(_settings.SmtpHost)
            {
                Port = _settings.SmtpPort,
                Credentials = new NetworkCredential(
                    _settings.SenderEmail,
                    _settings.Password),
                EnableSsl = true
            };

            var body = $@"
                <h2>NOVA Store</h2>
                <p>Hello {customerName}</p>

                <p>Your order has been placed successfully.</p>

                <p><strong>Order ID:</strong> {orderId}</p>
                <p><strong>Total:</strong> ${total}</p>

                <p>Thank you for shopping with us.</p>
            ";

            var mail = new MailMessage
            {
                From = new MailAddress(
                    _settings.SenderEmail,
                    _settings.SenderName),
                Subject = $"Order #{orderId} Confirmation",
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(customerEmail);

            await smtp.SendMailAsync(mail);
        }
    }
}