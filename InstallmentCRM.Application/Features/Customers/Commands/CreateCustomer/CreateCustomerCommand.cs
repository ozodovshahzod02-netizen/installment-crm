using MediatR;

namespace InstallmentCRM.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<Guid>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string PassportNumber { get; set; } = string.Empty;
}