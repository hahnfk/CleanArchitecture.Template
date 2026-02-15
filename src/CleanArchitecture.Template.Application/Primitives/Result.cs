namespace CleanArchitecture.Template.Application.Primitives;

public readonly record struct Error(string Code, string Message);

public readonly record struct Result<T>(T? Value, bool IsSuccess, Error? Error)
{
    public static Result<T> Ok(T value) => new(value, true, null);
    public static Result<T> Fail(string code, string message) => new(default, false, new Error(code, message));
}
