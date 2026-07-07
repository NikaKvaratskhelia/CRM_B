using System.ComponentModel.DataAnnotations;

namespace CRM_B.Application.Options;

public sealed class ClientOptions
{
    public const string SectionName = "Client";

    [Required, Url] public string BaseUrl { get; set; } = string.Empty;
}