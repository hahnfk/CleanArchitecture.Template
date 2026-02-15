namespace CleanArchitecture.Template.Application.Customers.Dtos;

public sealed record CustomerListItemDto(
    Guid Id,
    string Name,
    string Email);
