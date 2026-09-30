using System.ComponentModel;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using ModelContextProtocol.Server;

namespace CarRental.MCP.Tools;

[McpServerToolType]
public static class CustomerTools
{
    [McpServerTool(Name = "get_customer"), Description("Gets a single customer by id.")]
    public static Task<CustomerDto> GetCustomer(ICustomerService customerService, int customerId, CancellationToken cancellationToken)
        => customerService.GetByIdAsync(customerId, cancellationToken);

    [McpServerTool(Name = "list_customers"), Description("Lists all customers.")]
    public static Task<IReadOnlyList<CustomerDto>> ListCustomers(ICustomerService customerService, CancellationToken cancellationToken)
        => customerService.GetAllAsync(cancellationToken);

    [McpServerTool(Name = "create_customer"), Description("Registers a new customer.")]
    public static Task<CustomerDto> CreateCustomer(ICustomerService customerService, CreateCustomerRequest request, CancellationToken cancellationToken)
        => customerService.CreateAsync(request, cancellationToken);
}
