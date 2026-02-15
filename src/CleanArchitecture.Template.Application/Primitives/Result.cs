namespace CleanArchitecture.Template.Application.Primitives;

public readonly record struct Error(string Code, string Message)
{
    public static readonly Error None = new("", "");
    public bool IsNone => string.IsNullOrWhiteSpace(Code);
    public override string ToString() => IsNone ? "None" : $"{Code}: {Message}";
}

public readonly struct Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = isSuccess ? Error.None : error;
    }

    public static Result Ok() => new(true, Error.None);

    public static Result Fail(Error error) => new(false, error);

    public static Result Fail(string code, string message) => new(false, new Error(code, message));
}

public readonly struct Result<T>
{
    private readonly T? _value;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value for a failed Result.");

    private Result(T? value, bool isSuccess, Error error)
    {
        _value = value;
        IsSuccess = isSuccess;
        Error = isSuccess ? Error.None : error;
    }

    public static Result<T> Ok(T value) => new(value, true, Error.None);

    public static Result<T> Fail(Error error) => new(default, false, error);

    public static Result<T> Fail(string code, string message) => new(default, false, new Error(code, message));
}
