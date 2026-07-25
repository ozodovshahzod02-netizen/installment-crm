using InstallmentCRM.Application.DTOs;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Dashboard.Queries.GetTopDebtors;

public class GetTopDebtorsQueryHandler
    : IRequestHandler<GetTopDebtorsQuery, List<TopDebtorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTopDebtorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TopDebtorDto>> Handle(
        GetTopDebtorsQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var contracts = await _context.InstallmentContracts
            .Include(x => x.Customer)
            .Include(x => x.PaymentSchedules)
            .Include(x => x.Payments)
            .ToListAsync(cancellationToken);

        var result = contracts
            .Select(contract =>
            {
                var paidAmount = contract.Payments.Sum(x => x.Amount);

                var remainingDebt = contract.TotalAmount - paidAmount;

                var overdueSchedules = contract.PaymentSchedules
                    .Where(x => !x.IsPaid && x.DueDate.Date < today)
                    .ToList();

                var daysLate = overdueSchedules.Any()
                    ? overdueSchedules.Max(x => (today - x.DueDate.Date).Days)
                    : 0;

                return new TopDebtorDto
                {
                    CustomerId = contract.CustomerId,
                    CustomerName = $"{contract.Customer.FirstName} {contract.Customer.LastName}",
                    PhoneNumber = contract.Customer.PhoneNumber,
                    ContractNumber = contract.ContractNumber,
                    RemainingDebt = remainingDebt,
                    OverduePayments = overdueSchedules.Count,
                    DaysLate = daysLate
                };
            })
            .Where(x => x.RemainingDebt > 0)
            .OrderByDescending(x => x.RemainingDebt)
            .ThenByDescending(x => x.DaysLate)
            .Take(request.Count)
            .ToList();

        return result;
    }
}