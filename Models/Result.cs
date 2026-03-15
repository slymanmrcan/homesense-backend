namespace Models;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public Error? Error { get; protected set; }

    public static Result Success() => new()
    {
        IsSuccess = true
    };

    public static Result Fail(Error error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public new static Result<T> Fail(Error error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}