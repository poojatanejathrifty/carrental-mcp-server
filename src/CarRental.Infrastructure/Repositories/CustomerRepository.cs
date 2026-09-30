using CarRental.Application.Interfaces.Repositories;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;

namespace CarRental.Infrastructure.Repositories;

public class CustomerRepository(ISqlDataAccess dataAccess) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(int customerId, CancellationToken cancellationToken = default)
        => dataAccess.QuerySingleOrDefaultAsync<Customer>(
            StoredProcedures.Customer.GetById,
            new { CustomerId = customerId },
            cancellationToken);

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => dataAccess.QuerySingleOrDefaultAsync<Customer>(
            StoredProcedures.Customer.GetByEmail,
            new { Email = email },
            cancellationToken);

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dataAccess.QueryAsync<Customer>(StoredProcedures.Customer.GetAll, cancellationToken: cancellationToken);

    public Task<int> CreateAsync(Customer customer, CancellationToken cancellationToken = default)
        => dataAccess.ExecuteScalarAsync<int>(
            StoredProcedures.Customer.Create,
            new
            {
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.PhoneNumber,
                customer.DriverLicenseNumber,
                customer.DateOfBirth,
                customer.CreatedAtUtc
            },
            cancellationToken)!;

    public async Task<bool> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        var rows = await dataAccess.ExecuteAsync(
            StoredProcedures.Customer.Update,
            new
            {
                customer.CustomerId,
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.PhoneNumber
            },
            cancellationToken);

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var rows = await dataAccess.ExecuteAsync(
            StoredProcedures.Customer.Delete,
            new { CustomerId = customerId },
            cancellationToken);

        return rows > 0;
    }
}
