using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Payments.Commands.DeletePayment;

public class DeletePaymentCommandHandler
    : IRequestHandler<DeletePaymentCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeletePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (payment is null)
            throw new NotFoundException("Payment not found.");

        var schedule = await _context.PaymentSchedules
            .Include(x => x.Contract)
            .FirstOrDefaultAsync(
                x => x.Id == payment.PaymentScheduleId,
                cancellationToken);

        if (schedule is not null)
        {
            schedule.PaidAmount -= payment.Amount;

            if (schedule.PaidAmount < 0)
                schedule.PaidAmount = 0;

            schedule.IsPaid = false;
            schedule.PaymentDate = null;

            schedule.SetUpdated();

            schedule.Contract.Status = ContractStatus.Active;
            schedule.Contract.SetUpdated();
        }

        _context.Payments.Remove(payment);

        await _context.SaveChangesAsync(cancellationToken);
    }
}