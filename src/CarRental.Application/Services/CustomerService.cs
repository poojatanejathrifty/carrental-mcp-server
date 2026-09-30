using CarRental.Application.Common;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Services;

public class CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<CustomerDto> GetByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), customerId);

        return ToDto(customer);
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(ToDto).ToList();
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await customerRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException($"A customer with email '{request.Email}' already exists.");
        }

        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DriverLicenseNumber = request.DriverLicenseNumber,
            DateOfBirth = request.DateOfBirth,
            CreatedAtUtc = DateTime.UtcNow
        };

        customer.CustomerId = await customerRepository.CreateAsync(customer, cancellationToken);
        logger.LogInformation("Created customer {CustomerId}", customer.CustomerId);

        return ToDto(customer);
    }

    public async Task<CustomerDto> UpdateAsync(UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), request.CustomerId);

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.PhoneNumber = request.PhoneNumber;

        await customerRepository.UpdateAsync(customer, cancellationToken);
        return ToDto(customer);
    }

    public async Task DeleteAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var deleted = await customerRepository.DeleteAsync(customerId, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException(nameof(Customer), customerId);
        }
    }

    private static CustomerDto ToDto(Customer customer) => new()
    {
        CustomerId = customer.CustomerId,
        FirstName = customer.FirstName,
        LastName = customer.LastName,
        Email = customer.Email,
        PhoneNumber = customer.PhoneNumber,
        DriverLicenseNumber = customer.DriverLicenseNumber,
        DateOfBirth = customer.DateOfBirth
    };
}
