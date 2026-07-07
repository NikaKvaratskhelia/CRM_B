using System.Collections.Concurrent;
using System.Reflection;
using CRM_B.Application.Abstractions.Email;
using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Localization;
using CRM_B.Domain.Aggregates.Auth.Errors;
using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Kernel.Results;
using FluentEmail.Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CRM_B.Infrastructure.Email.Sender;

public sealed class EmailSender(
    ITemplateRenderer renderer,
    IEmailLocalizer subjects,
    IOptions<MailKitOptions> options,
    ILogger<EmailSender> logger)
    : IEmailSender
{
    private static readonly Assembly TemplatesAssembly = typeof(EmailSender).Assembly;
    private static readonly string ResourcePrefix = $"{TemplatesAssembly.GetName().Name}.Email.Templates.";
    private static readonly ConcurrentDictionary<string, string> TemplateCache = new();
    private readonly MailKitOptions _options = options.Value;

    public async Task<Result> Send<T>(string to, string subjectCode, string template, Language language, T model)
    {
        try
        {
            var subject = subjects.Subject(subjectCode, language);
            var templateBody = await LoadTemplateAsync(template, language);
            var html = await renderer.ParseAsync(templateBody, model);

            // 1. Build the MimeKit Message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = html };
            message.Body = bodyBuilder.ToMessageBody();

            // 2. Connect and Send via MailKit SmtpClient
            using var client = new SmtpClient();

            // Use StartTls for port 587, or SSLOnConnect for port 465
            var secureSocket = _options.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            await client.ConnectAsync(_options.Host, _options.Port, secureSocket);
            await client.AuthenticateAsync(_options.Username, _options.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MailKit email transmission threw an exception for template: {Template}", template);
            return Result.Failure(AuthErrors.EmailSendFailed(ex.Message));
        }
    }

    private static async Task<string> LoadTemplateAsync(string template, Language language)
    {
        var suffix = CultureResolver.ToCultureName(language);
        var cacheKey = $"{template}|{suffix}";
        if (TemplateCache.TryGetValue(cacheKey, out var cached))
            return cached;

        var path = template.Replace('/', '.').Replace('\\', '.');
        var primaryName = $"{ResourcePrefix}{path}.{suffix}.cshtml";
        var fallbackName = $"{ResourcePrefix}{path}.en.cshtml";

        await using var stream =
            TemplatesAssembly.GetManifestResourceStream(primaryName)
            ?? TemplatesAssembly.GetManifestResourceStream(fallbackName)
            ?? throw new FileNotFoundException($"Email template '{template}' not found.");

        using var reader = new StreamReader(stream);
        var body = await reader.ReadToEndAsync();

        TemplateCache[cacheKey] = body;
        return body;
    }
}