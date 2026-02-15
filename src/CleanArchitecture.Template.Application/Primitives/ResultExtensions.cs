namespace CleanArchitecture.Template.Application.Primitives;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
        => result.IsSuccess ? Result<TOut>.Ok(map(result.Value)) : Result<TOut>.Fail(result.Error);

    public static async Task<Result<TOut>> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> bind)
        => result.IsSuccess ? await bind(result.Value) : Result<TOut>.Fail(result.Error);

    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
        => result.IsSuccess && !predicate(result.Value) ? Result<T>.Fail(error) : result;

    public static Result Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess) action(result.Value);
        return result.IsSuccess ? Result.Ok() : Result.Fail(result.Error);
    }

    public static Result<Unit> ToUnit<T>(this Result<T> result)
        => result.IsSuccess ? Result<Unit>.Ok(new Unit()) : Result<Unit>.Fail(result.Error);
}
