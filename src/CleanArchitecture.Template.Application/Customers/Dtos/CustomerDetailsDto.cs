namespace CleanArchitecture.Template.Application.Customers.Dtos;

public sealed record CustomerDetailsDto(
    Guid Id,
    string Name,
    string Email,
    bool IsActive);
