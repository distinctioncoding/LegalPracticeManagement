namespace LawFirm.Shared.Results;

public  record Error(string Code, string Message)
{
    public static Error NotFound(string entity, object key) =>
        new("not_found", $"{entity} '{key}' was not found.");

    public static Error Validation(string message) =>
        new("validation_error", message);

    public static Error Conflict(string message) =>
        new("conflict", message);

    public static Error Forbidden(string message) =>
        new("forbidden", message);
}
