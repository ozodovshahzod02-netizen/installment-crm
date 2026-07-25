using InstallmentCRM.Application.DTOs;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.PaymentSchedules.Queries.GetPaymentSchedulesByContract;

public class GetPaymentSchedulesByContractQueryHandler
    : IRequestHandler<GetPaymentSchedulesByContractQuery, List<PaymentScheduleDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPaymentSchedulesByContractQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentScheduleDto>> Handle(
        GetPaymentSchedulesByContractQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.PaymentSchedules
            .Where(x => x.ContractId == request.ContractId)
            .OrderBy(x => x.MonthNumber)
            .Select(x => new PaymentScheduleDto
            {
                Id = x.Id,
                ContractId = x.ContractId,
                MonthNumber = x.MonthNumber,
                ExpectedAmount = x.ExpectedAmount,
                PaidAmount = x.PaidAmount,
                RemainingAmount = x.ExpectedAmount - x.PaidAmount,
                DueDate = x.DueDate,
                PaymentDate = x.PaymentDate,
                IsPaid = x.IsPaid
            })
            .ToListAsync(cancellationToken);
    }
}