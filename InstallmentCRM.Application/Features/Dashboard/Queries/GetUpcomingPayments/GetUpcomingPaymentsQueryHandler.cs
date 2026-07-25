using InstallmentCRM.Application.DTOs;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Dashboard.Queries.GetUpcomingPayments;

public class GetUpcomingPaymentsQueryHandler
    : IRequestHandler<GetUpcomingPaymentsQuery, List<UpcomingPaymentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUpcomingPaymentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UpcomingPaymentDto>> Handle(
        GetUpcomingPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var endDate = today.AddDays(request.Days);

        return await _context.PaymentSchedules
            .Include(x => x.Contract)
                .ThenInclude(x => x.Customer)
            .Where(x =>
                !x.IsPaid &&
                x.DueDate.Date >= today &&
                x.DueDate.Date <= endDate)
            .OrderBy(x => x.DueDate)
            .Select(x => new UpcomingPaymentDto
            {
                ScheduleId = x.Id,
                ContractId = x.ContractId,
                ContractNumber = x.Contract.ContractNumber,
                CustomerName = x.Contract.Customer.FirstName + " " + x.Contract.Customer.LastName,
                PhoneNumber = x.Contract.Customer.PhoneNumber,
                Amount = x.ExpectedAmount - x.PaidAmount,
                DueDate = x.DueDate,
                DaysLeft = (x.DueDate.Date - today).Days
            })
            .ToListAsync(cancellationToken);
    }
}