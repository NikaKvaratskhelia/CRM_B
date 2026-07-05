namespace CRM_B.Domain.Kernel.Results.Errors;

public enum ErrorKind
{
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    Internal
}