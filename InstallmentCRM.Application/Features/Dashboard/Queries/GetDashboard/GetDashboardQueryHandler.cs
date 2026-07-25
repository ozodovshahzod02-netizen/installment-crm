using InstallmentCRM.Application.DTOs;
using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Dashboard.Queries.GetDashboard;

public class GetDashboardQueryHandler
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow.Date;

        var dto = new DashboardDto();

        dto.TotalCustomers =
            await _context.Customers.CountAsync(cancellationToken);

        dto.TotalProducts =
            await _context.Products.CountAsync(cancellationToken);

        dto.TotalContracts =
            await _context.InstallmentContracts.CountAsync(cancellationToken);

        dto.ActiveContracts =
            await _context.InstallmentContracts.CountAsync(
                x => x.Status == ContractStatus.Active,
                cancellationToken);

        dto.CompletedContracts =
            await _context.InstallmentContracts.CountAsync(
                x => x.Status == ContractStatus.Completed,
                cancellationToken);

        dto.TotalContractAmount =
            await _context.InstallmentContracts
                .SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0;

        dto.TotalPaidAmount =
            await _context.Payments
                .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;

        dto.RemainingAmount =
            dto.TotalContractAmount - dto.TotalPaidAmount;

        dto.PaidSchedules =
            await _context.PaymentSchedules
                .CountAsync(x => x.IsPaid, cancellationToken);

        dto.OverdueSchedules =
            await _context.PaymentSchedules
                .CountAsync(x =>
                    !x.IsPaid &&
                    x.DueDate.Date < now,
                    cancellationToken);

        dto.DueTodaySchedules =
            await _context.PaymentSchedules
                .CountAsync(x =>
                    !x.IsPaid &&
                    x.DueDate.Date == now,
                    cancellationToken);

        return dto;
    }
}