using System.Linq.Expressions;

namespace CleanArchitecture.Template.Application.Abstractions.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    int? Skip { get; }
    int? Take { get; }
    bool AsNoTracking { get; }
}
