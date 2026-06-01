namespace Soundmates.IntegrationTests.Common.Email;

internal enum CapturedEmailKind
{
    RegistrationConfirmation,
    PasswordReset,
    Generic
}

/// <summary>
/// A single email recorded by <see cref="CapturingEmailService"/>.
/// For confirmation/reset emails <see cref="Link"/> holds the full client link (with ?token=);
/// for generic emails <see cref="Subject"/> and <see cref="Body"/> are populated.
/// </summary>
internal sealed record CapturedEmail(
    CapturedEmailKind Kind,
    string Email,
    string? Link,
    string? Subject,
    string? Body)
{
    /// <summary>Extracts the raw value of the "token" query parameter from <see cref="Link"/>.</summary>
    public string? Token
    {
        get
        {
            if (Link is null)
            {
                return null;
            }

            var query = new Uri(Link).Query.TrimStart('?');
            foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var separatorIndex = pair.IndexOf('=', StringComparison.Ordinal);
                if (separatorIndex < 0)
                {
                    continue;
                }

                var key = pair[..separatorIndex];
                if (key.Equals("token", StringComparison.OrdinalIgnoreCase))
                {
                    return Uri.UnescapeDataString(pair[(separatorIndex + 1)..]);
                }
            }

            return null;
        }
    }
}
