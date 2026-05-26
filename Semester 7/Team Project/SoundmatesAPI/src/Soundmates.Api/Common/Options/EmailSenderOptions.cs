using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.Common.Options;

internal sealed class EmailSenderOptions
{
    public const string SectionName = "EmailSender";

    [Required]
    public required string SmtpServer { get; set; }

    [Required, Range(1, 65535)]
    public required int Port { get; set; }

    [Required]
    public required string SenderEmail { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string DisplayName { get; set; }

    [Required]
    public required bool UseStubEmailSender { get; set; }
}

