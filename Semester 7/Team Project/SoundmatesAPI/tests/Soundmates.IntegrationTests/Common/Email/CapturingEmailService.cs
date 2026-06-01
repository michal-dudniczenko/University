using Soundmates.Api.Common.Services;
using System.Collections.Concurrent;

namespace Soundmates.IntegrationTests.Common.Email;

/// <summary>
/// Test double for <see cref="IEmailService"/> that records every email instead of sending it.
/// Registered as a singleton in <see cref="CustomWebApplicationFactory"/> so captured messages
/// can be asserted across the request boundary. Cleared per test via <see cref="Clear"/>.
/// </summary>
internal sealed class CapturingEmailService : IEmailService
{
    private readonly ConcurrentQueue<CapturedEmail> _sentEmails = new();

    public IReadOnlyList<CapturedEmail> SentEmails => _sentEmails.ToArray();

    public void Clear() => _sentEmails.Clear();

    public Task SendRegistrationConfirmationLinkAsync(
        string email,
        string link,
        CancellationToken cancellationToken = default)
    {
        _sentEmails.Enqueue(new CapturedEmail(
            CapturedEmailKind.RegistrationConfirmation, email, link, Subject: null, Body: null));
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(
        string email,
        string link,
        CancellationToken cancellationToken = default)
    {
        _sentEmails.Enqueue(new CapturedEmail(
            CapturedEmailKind.PasswordReset, email, link, Subject: null, Body: null));
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(
        string email,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _sentEmails.Enqueue(new CapturedEmail(
            CapturedEmailKind.Generic, email, Link: null, subject, body));
        return Task.CompletedTask;
    }
}
