using MailKit.Net.Smtp;
using MimeKit;
using System.Globalization;

namespace Soundmates.Api.Common.Services;

internal interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

internal sealed class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var port = emailSettings["Port"];
            var senderEmail = emailSettings["SenderEmail"]
                ?? throw new InvalidOperationException("EmailSettings::SenderEmail is not configured.");
            var password = emailSettings["Password"]
                ?? throw new InvalidOperationException("EmailSettings::Password is not configured.");
            var displayName = emailSettings["DisplayName"] ?? "Soundmates";

            if (string.IsNullOrEmpty(smtpServer) || smtpServer == "smtp.example.com")
            {
                logger.LogWarning("Email settings are not configured. Email to {To} with subject '{Subject}' was not sent.", to, subject);
                logger.LogInformation("Email body (not sent): {Body}", body);
                return;
            }

            using var email = new MimeMessage();
            email.From.Add(new MailboxAddress(displayName, senderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, int.Parse(port ?? "587", CultureInfo.InvariantCulture), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderEmail, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            logger.LogInformation("Email sent successfully to {To} with subject '{Subject}'", to, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'", to, subject);
            logger.LogInformation("Email body (failed to send): {Body}", body);
        }
    }
}
