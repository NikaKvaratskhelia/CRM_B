using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Kernel.Results;

namespace CRM_B.Application.Abstractions.Email;

public interface IEmailSender
{
    Task<Result> Send<T>(string to, string subjectCode, string template, Language language, T model);
}