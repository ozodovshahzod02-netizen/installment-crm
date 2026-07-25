using InstallmentCRM.Application.DTOs;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Customers.Queries;

public class GetAllCustomersQueryHandler
    : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllCustomersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(
        GetAllCustomersQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Customers
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                PassportNumber = customer.PassportNumber
            })
            .ToListAsync(cancellationToken);
    }
}