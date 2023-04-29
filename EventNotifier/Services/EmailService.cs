using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace EventNotifier.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _options;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger) {
            _options = options ?? throw new ArgumentNullException(nameof(_options)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }
        public async Task SendMessageAsync(string emailName, string title, string htmlMessage)
        {
            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_options.Value.EmailFrom));
            email.To.Add(MailboxAddress.Parse(emailName));
            email.Subject = title;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            // send email
            using (var smtp = new SmtpClient())
            {
               await smtp.ConnectAsync(_options.Value.SmtpHost, _options.Value.SmtpPort, SecureSocketOptions.StartTls);
               await smtp.AuthenticateAsync(_options.Value.Username, _options.Value.Password);
               await smtp.SendAsync(email);
               await smtp.DisconnectAsync(true);
            }
        }
    }


    public class EmailSettings
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SmtpHost { get; set; } = null!;
        public int SmtpPort { get; set; }
        public string EmailFrom { get; set; } = null!;
    }
}
