using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Soundmates.Api.Common.Options;

namespace Soundmates.Api.Common.Services;

internal interface IEmailService
{
    Task SendRegistrationConfirmationLinkAsync(string email, string link, CancellationToken cancellationToken = default);
    Task SendPasswordResetLinkAsync(string email, string link, CancellationToken cancellationToken = default);
    Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken = default);
}

internal sealed class EmailService(
    IOptions<EmailSenderOptions> emailSenderOptions,
    ILogger<EmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(
        string email,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var emailSettings = emailSenderOptions.Value;

            using var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(emailSettings.DisplayName, emailSettings.SenderEmail));
            mimeMessage.To.Add(MailboxAddress.Parse(email));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                emailSettings.SmtpServer,
                emailSettings.Port,
                MailKit.Security.SecureSocketOptions.StartTls,
                cancellationToken);
            await smtp.AuthenticateAsync(emailSettings.SenderEmail, emailSettings.Password, cancellationToken);
            await smtp.SendAsync(mimeMessage, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email sent successfully. Subject: '{Subject}'", subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email. Subject: '{Subject}'", subject);
            throw;
        }
    }

    public Task SendPasswordResetLinkAsync(
        string email,
        string link,
        CancellationToken cancellationToken = default)
    {
        const string subject = "Reset your Soundmates password";
        var body = $"""
            <h1>Password Reset</h1>
            <p>You requested a password reset for your Soundmates account.</p>
            <p>Click the link below to set a new password:</p>
            <p><a href="{link}">Reset Password</a></p>
            <p>If you did not request this, you can safely ignore this email.</p>
            <p>This link will expire shortly for your security.</p>
            """;

        return SendEmailAsync(email, subject, body, cancellationToken);
    }

    public Task SendRegistrationConfirmationLinkAsync(
        string email,
        string link,
        CancellationToken cancellationToken = default)
    {
        const string subject = "Confirm your Soundmates account";
        var body = $"""
            <h1>Welcome to Soundmates!</h1>
            <p>Thank you for registering. Please confirm your email address by clicking the link below:</p>
            <p><a href="{link}">Confirm Email</a></p>
            <p>If you did not create an account, you can safely ignore this email.</p>
            """;

        return SendEmailAsync(email, subject, body, cancellationToken);
    }
}
