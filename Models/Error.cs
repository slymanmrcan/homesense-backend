namespace Models;

public class Error
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;

    public static Error Validation(string message) => new()
    {
        Code = "validation_error",
        Message = message
    };

    public static Error NotFound(string message) => new()
    {
        Code = "not_found",
        Message = message
    };

    public static Error Conflict(string message) => new()
    {
        Code = "conflict",
        Message = message
    };

    public static Error Unauthorized(string message) => new()
    {
        Code = "unauthorized",
        Message = message
    };

    public static Error Failure(string message) => new()
    {
        Code = "failure",
        Message = message
    };
}