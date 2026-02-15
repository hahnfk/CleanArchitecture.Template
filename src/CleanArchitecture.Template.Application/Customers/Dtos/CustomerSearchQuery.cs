namespace CleanArchitecture.Template.Application.Customers.Dtos;

public sealed record CustomerSearchQuery(
    string? Term,
    int Page = 1,
    int PageSize = 20);
