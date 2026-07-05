namespace CRM_B.Application.Abstractions.Localization;

public interface IErrorLocalizer
{
    string this[string code] { get; }
    string Get(string code, params object?[] args);
}