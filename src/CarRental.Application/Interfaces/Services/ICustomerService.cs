using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;

namespace CarRental.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<CustomerDto> GetByIdAsync(int customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<CustomerDto> UpdateAsync(UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int customerId, CancellationToken cancellationToken = default);
}
