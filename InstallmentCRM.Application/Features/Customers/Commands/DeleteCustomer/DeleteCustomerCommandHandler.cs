using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler
    : IRequestHandler<DeleteCustomerCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCustomerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException("Customer not found.");
        }

        var hasContracts = await _context.InstallmentContracts
            .AnyAsync(c => c.CustomerId == customer.Id, cancellationToken);

        if (hasContracts)
        {
            throw new ValidationException(
                "Cannot delete customer because they have installment contracts.");
        }

        _context.Customers.Remove(customer);

        await _context.SaveChangesAsync(cancellationToken);
    }
}