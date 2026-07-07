using System.ComponentModel.DataAnnotations;

namespace CRM_B.Infrastructure.Email.Sender;

public sealed class MailKitOptions
{
    public const string SectionName = "MailKit";

    [Required] public string Host { get; init; } = string.Empty;

    [Required] public int Port { get; init; }

    [Required] public string Username { get; init; } = string.Empty;

    [Required] public string Password { get; init; } = string.Empty;
    [Required, EmailAddress] public string FromEmail { get; init; } = string.Empty;

    [Required] public string FromName { get; init; } = string.Empty;
}