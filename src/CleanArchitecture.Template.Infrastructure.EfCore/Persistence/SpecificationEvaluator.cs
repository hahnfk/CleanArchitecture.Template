using CleanArchitecture.Template.Application.Abstractions.Specifications;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Template.Infrastructure.EfCore.Persistence;

internal static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> spec) where T : class
    {
        var query = inputQuery;

        if (spec.AsNoTracking)
            query = query.AsNoTracking();

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        foreach (var include in spec.Includes)
            query = query.Include(include);

        if (spec.OrderBy is not null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending is not null)
            query = query.OrderByDescending(spec.OrderByDescending);

        if (spec.Skip is not null)
            query = query.Skip(spec.Skip.Value);

        if (spec.Take is not null)
            query = query.Take(spec.Take.Value);

        return query;
    }
}
