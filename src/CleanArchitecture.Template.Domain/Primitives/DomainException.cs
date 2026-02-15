namespace CleanArchitecture.Template.Domain.Primitives;

public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
