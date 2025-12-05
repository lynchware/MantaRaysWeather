using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

namespace MantaRays_Weather.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly AuthMessageSenderOptions _options;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
    {
        _options = optionsAccessor.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            if (string.IsNullOrEmpty(_options.SmtpHost))
            {
                throw new InvalidOperationException("SMTP configuration is not set. Please configure email settings.");
            }

            await Execute(_options.SmtpHost, _options.SmtpPort, _options.SmtpUser ?? "", 
                _options.SmtpPassword ?? "", _options.FromEmail ?? "", _options.FromName ?? "", email, subject, htmlMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", email);
            throw;
        }
    }

    private async Task Execute(string smtpHost, int smtpPort, string smtpUser, 
        string smtpPassword, string fromEmail, string fromName, string toEmail, 
        string subject, string htmlMessage)
    {
        using (var client = new SmtpClient())
        {
            // Connect to the SMTP server
            await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            // Authenticate with the SMTP server
            if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword))
            {
                await client.AuthenticateAsync(smtpUser, smtpPassword);
            }

            // Create the email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName ?? "unknown sender", fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            // Create the HTML body
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            message.Body = bodyBuilder.ToMessageBody();

            // Send the email
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email to {EmailAddress} sent successfully with subject '{Subject}'", toEmail, subject);
        }
    }
}
