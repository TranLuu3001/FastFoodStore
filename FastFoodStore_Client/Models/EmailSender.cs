using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace FastFoodStore_Client.Models
{
    public class EmailSender:IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                // Cấu hình SMTP
                var host = _configuration["EmailSettings:Host"] ?? throw new InvalidOperationException("Host is not configured.");
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? throw new InvalidOperationException("Port is not configured."));
                var username = _configuration["EmailSettings:Username"] ?? throw new InvalidOperationException("Username is not configured.");
                var password = _configuration["EmailSettings:Password"] ?? throw new InvalidOperationException("Password is not configured.");
                var from = _configuration["EmailSettings:From"] ?? throw new InvalidOperationException("From address is not configured.");

                var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                // Chuẩn bị email
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from, "Hỗ trợ khách hàng"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                // Gửi email tới người dùng (toEmail)
                mailMessage.To.Add(toEmail);

                // Gửi email
                await smtpClient.SendMailAsync(mailMessage);

                Console.WriteLine("Email đã được gửi thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
                throw;
            }
        }   
    }
}
