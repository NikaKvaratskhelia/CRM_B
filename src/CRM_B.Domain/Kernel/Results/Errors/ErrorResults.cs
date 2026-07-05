namespace CRM_B.Domain.Kernel.Results.Errors;

public sealed record ErrorResults(string Code, params object?[] Args)
{
    public static readonly ErrorResults InsufficientPermissions = new("common.insufficient_permissions")
        { Kind = ErrorKind.Forbidden };

    public static readonly ErrorResults Unauthorized = new("common.unauthorized") { Kind = ErrorKind.Unauthorized };
    public static readonly ErrorResults InternalError = new("common.internal_error") { Kind = ErrorKind.Internal };
    public static readonly ErrorResults AlreadyExists = new("common.already_exists") { Kind = ErrorKind.Conflict };
    public static readonly ErrorResults DoesNotExist = new("common.does_not_exist") { Kind = ErrorKind.NotFound };
    public static readonly ErrorResults Forbidden = new("common.forbidden") { Kind = ErrorKind.Forbidden };
    public static readonly ErrorResults ValidationFailed = new("common.validation_failed");

    public static readonly ErrorResults IdempotencyConflict = new("common.idempotency_conflict")
        { Kind = ErrorKind.Conflict };

    public ErrorKind Kind { get; init; } = ErrorKind.Validation;
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

    public static ErrorResults Validation(IReadOnlyDictionary<string, string[]> errors) =>
        new("common.validation_failed") { Errors = errors };

    public static ErrorResults NotFound(string entity) => new("common.not_found", entity) { Kind = ErrorKind.NotFound };
    public static ErrorResults Conflict(string reason) => new("common.conflict", reason) { Kind = ErrorKind.Conflict };

    public static ErrorResults Required(string field) => new("field.required", field);

    public static ErrorResults InvalidLength(string field, int min, int max) =>
        new("field.invalid_length", field, min, max);

    public static ErrorResults InvalidFormat(string field) => new("field.invalid_format", field);
    public static ErrorResults Invalid(string field) => new("field.invalid", field);

    public static ErrorResults MustBeGreaterThan(string field, int value) =>
        new("field.must_be_greater_than", field, value);
};