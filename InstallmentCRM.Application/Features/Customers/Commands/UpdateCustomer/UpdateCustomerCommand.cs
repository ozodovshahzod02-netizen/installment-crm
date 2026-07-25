using MediatR;

namespace InstallmentCRM.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string PassportNumber { get; set; } = string.Empty;
}