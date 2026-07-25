using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Payments.Commands.UpdatePayment;

public class UpdatePaymentCommandHandler
    : IRequestHandler<UpdatePaymentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdatePaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        UpdatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .Include(x => x.PaymentSchedule)
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (payment is null)
            throw new NotFoundException("Payment not found.");

        if (request.Amount <= 0)
            throw new ValidationException("Payment amount must be greater than zero.");

        var schedule = payment.PaymentSchedule;

        // Пересчитываем PaidAmount графика: убираем старую сумму, применяем новую
        var paidAmountWithoutThisPayment = schedule.PaidAmount - payment.Amount;

        if (paidAmountWithoutThisPayment < 0)
            paidAmountWithoutThisPayment = 0;

        var newPaidAmount = paidAmountWithoutThisPayment + request.Amount;

        if (newPaidAmount > schedule.ExpectedAmount)
        {
            var maxAllowed = schedule.ExpectedAmount - paidAmountWithoutThisPayment;

            throw new ValidationException(
                $"Maximum payment amount is {maxAllowed}.");
        }

        payment.Amount = request.Amount;
        payment.PaymentMethod = request.PaymentMethod;
        payment.Notes = request.Notes;

        payment.SetUpdated();

        schedule.PaidAmount = newPaidAmount;

        if (schedule.PaidAmount >= schedule.ExpectedAmount)
        {
            schedule.PaidAmount = schedule.ExpectedAmount;
            schedule.IsPaid = true;
            schedule.PaymentDate = payment.PaymentDate;
        }
        else
        {
            schedule.IsPaid = false;
            schedule.PaymentDate = null;
        }

        schedule.SetUpdated();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
