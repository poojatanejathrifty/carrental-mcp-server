using CarRental.Domain.Entities;

namespace CarRental.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int customerId, CancellationToken cancellationToken = default);
}
