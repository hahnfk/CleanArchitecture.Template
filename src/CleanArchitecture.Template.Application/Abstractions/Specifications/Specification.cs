using System.Linq.Expressions;

namespace CleanArchitecture.Template.Application.Abstractions.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected init; }

    private readonly List<Expression<Func<T, object>>> _includes = new();
    public IReadOnlyList<Expression<Func<T, object>>> Includes => _includes;

    public Expression<Func<T, object>>? OrderBy { get; protected init; }
    public Expression<Func<T, object>>? OrderByDescending { get; protected init; }

    public int? Skip { get; protected init; }
    public int? Take { get; protected init; }

    public bool AsNoTracking { get; protected init; } = true;

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
        => _includes.Add(includeExpression);
}
