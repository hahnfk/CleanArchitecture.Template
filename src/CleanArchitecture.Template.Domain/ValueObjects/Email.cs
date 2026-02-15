using System.Text.RegularExpressions;
using CleanArchitecture.Template.Domain.Primitives;

namespace CleanArchitecture.Template.Domain.ValueObjects;

public sealed record Email
{
    private static readonly Regex Pattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email must not be empty.");

        value = value.Trim();

        if (!Pattern.IsMatch(value))
            throw new DomainException("Email is not in a valid format.");

        return new Email(value);
    }

    public override string ToString() => Value;
}
