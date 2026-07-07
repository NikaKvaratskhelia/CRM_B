using CRM_B.Domain.Aggregates.Users.Enums;

namespace CRM_B.Application.Abstractions.Localization;

public interface IEmailLocalizer
{
    string Subject(string code, Language language, params object?[] args);
}