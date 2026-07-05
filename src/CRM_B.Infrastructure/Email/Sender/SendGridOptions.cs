using System.ComponentModel.DataAnnotations;

namespace CRM_B.Infrastructure.Email.Sender;

public sealed class SendGridOptions
{
    public const string SectionName = "SendGrid";

    [Required] public string Key { get; init; } = string.Empty;

    [Required, EmailAddress] public string FromEmail { get; init; } = string.Empty;

    [Required] public string FromName { get; init; } = string.Empty;
}