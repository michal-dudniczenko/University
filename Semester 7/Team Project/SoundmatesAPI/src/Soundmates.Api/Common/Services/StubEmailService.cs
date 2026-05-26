namespace Soundmates.Api.Common.Services;

internal sealed class StubEmailService(ILogger<StubEmailService> logger) : IEmailService
{
    public Task SendEmailAsync(
        string email,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[DEV EMAIL] Subject: {Subject}, for {Email}. \nBody: {Body}", subject, email, body);
        return Task.CompletedTask;
    }

    public Task SendRegistrationConfirmationLinkAsync(string email, string link, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[DEV EMAIL] Email Confirmation: for {Email}. Link: {Link}", email, link);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(string email, string link, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[DEV EMAIL] Password Reset: for {Email}. Link: {Link}", email, link);
        return Task.CompletedTask;
    }
}
