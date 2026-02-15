namespace CleanArchitecture.Template.Application.Abstractions.Queries;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize);
