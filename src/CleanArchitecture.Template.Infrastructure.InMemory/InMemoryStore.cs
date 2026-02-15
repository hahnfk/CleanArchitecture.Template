using System.Collections.Concurrent;
using CleanArchitecture.Template.Domain.Aggregates;

namespace CleanArchitecture.Template.Infrastructure.InMemory;

public sealed class InMemoryStore
{
    private readonly ConcurrentDictionary<Guid, Customer> _customers = new();

    public Customer? TryGetCustomer(Guid id)
        => _customers.TryGetValue(id, out var c) ? c : null;

    public IReadOnlyCollection<Customer> Customers => (IReadOnlyCollection<Customer>)_customers.Values;

    public void Upsert(Customer customer) => _customers[customer.Id] = customer;

    public bool TryRemove(Guid id, out Customer? customer)
    {
        if (_customers.TryRemove(id, out var c))
        {
            customer = c;
            return true;
        }

        customer = null;
        return false;
    }
}
