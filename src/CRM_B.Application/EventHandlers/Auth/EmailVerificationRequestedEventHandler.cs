using CRM_B.Application.Abstractions.Email;
using CRM_B.Application.Emails.Auth;
using CRM_B.Domain.Aggregates.Auth.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM_B.Application.EventHandlers.Auth;

public sealed class EmailVerificationRequestedEventHandler(
    IEmailSender emailSender,
    ILogger<EmailVerificationRequestedEventHandler> logger)
    : INotificationHandler<EmailVerificationRequestedEvent>
{
    private const string VerificationUrlBase = "https://yourapp.com/verify-email"; // TODO: move to config

    public async Task Handle(EmailVerificationRequestedEvent notification, CancellationToken ct)
    {
        var verificationUrl =
            $"{VerificationUrlBase}?token={notification.Token}&email={Uri.EscapeDataString(notification.Email)}";

        var model = new EmailVerificationEmailModel(
            notification.FullName,
            notification.Otp,
            verificationUrl);

        var result = await emailSender.Send(
            to: notification.Email,
            subjectCode: "auth.email_verification",
            template: "Auth/EmailVerification",
            language: notification.Language,
            model: model);

        if (result.IsFailure)
        {
            logger.LogError(
                "Failed to send verification email to {Email} for user {UserId}: {Error}",
                notification.Email, notification.UserId, result.Error?.Code);
        }

        logger.LogInformation("gaigzavna");
    }
}