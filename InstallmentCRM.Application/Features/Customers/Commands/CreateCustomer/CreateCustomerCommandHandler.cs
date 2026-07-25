using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Domain.Entities;
using MediatR;

namespace InstallmentCRM.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCustomerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            PassportNumber = request.PassportNumber
        };

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}