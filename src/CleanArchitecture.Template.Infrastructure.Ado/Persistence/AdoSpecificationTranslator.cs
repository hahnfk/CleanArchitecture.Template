using System.Linq.Expressions;
using CleanArchitecture.Template.Application.Abstractions.Specifications;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Infrastructure.Ado.Persistence;

internal static class AdoSpecificationTranslator
{
    // Minimal translator for the template.
    // Supports expressions produced by ActiveCustomersSearchSpec:
    // x => x.IsActive && (x.Name.Contains(term) || x.Email.Value.Contains(term))
    public static (string WhereSql, Dictionary<string, object?> Parameters) Translate(ISpecification<Customer> spec)
    {
        if (spec.Criteria is null)
            return (string.Empty, new Dictionary<string, object?>());

        var parameters = new Dictionary<string, object?>();

        var term = TryExtractContainsTerm(spec.Criteria.Body);
        if (term is null)
            return ("IsActive = 1", parameters);

        parameters["$term"] = $"%{term}%";
        return ("IsActive = 1 AND (Name LIKE $term OR Email LIKE $term)", parameters);
    }

    private static string? TryExtractContainsTerm(Expression body)
    {
        var contains = FindContainsCall(body);
        if (contains is null) return null;

        var arg = contains.Arguments.Count == 1 ? contains.Arguments[0] : null;
        if (arg is null) return null;

        if (arg is MemberExpression me && me.Expression is ConstantExpression ce)
        {
            var value = me.Member switch
            {
                System.Reflection.FieldInfo fi => fi.GetValue(ce.Value),
                System.Reflection.PropertyInfo pi => pi.GetValue(ce.Value),
                _ => null
            };

            return value?.ToString();
        }

        if (arg is ConstantExpression c && c.Value is string s)
            return s;

        return null;
    }

    private static MethodCallExpression? FindContainsCall(Expression expr)
    {
        if (expr is MethodCallExpression m && m.Method.Name == nameof(string.Contains))
            return m;

        if (expr is BinaryExpression b)
            return FindContainsCall(b.Left) ?? FindContainsCall(b.Right);

        if (expr is UnaryExpression u)
            return FindContainsCall(u.Operand);

        return null;
    }
}
