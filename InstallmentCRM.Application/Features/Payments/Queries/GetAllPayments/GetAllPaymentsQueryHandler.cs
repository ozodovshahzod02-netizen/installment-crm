using InstallmentCRM.Application.DTOs;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Payments.Queries.GetAllPayments;

public class GetAllPaymentsQueryHandler
    : IRequestHandler<GetAllPaymentsQuery, List<PaymentDto>>
{
    private readonly IApplicationDbContext _context;


    public GetAllPaymentsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<PaymentDto>> Handle(
        GetAllPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _context.Payments

            .Include(x => x.Contract)

            .OrderByDescending(x => x.PaymentDate)

            .Select(x => new PaymentDto
            {
                Id = x.Id,

                ContractId = x.ContractId,

                PaymentScheduleId = x.PaymentScheduleId,

                ContractNumber =
                    x.Contract.ContractNumber,

                Amount = x.Amount,

                PaymentDate = x.PaymentDate,

                PaymentMethod = x.PaymentMethod,

                Notes = x.Notes
            })

            .ToListAsync(cancellationToken);


        return payments;
    }
}