using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Domain.Entities;
using InstallmentCRM.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandHandler
    : IRequestHandler<CreatePaymentCommand, Guid>
{
    private readonly IApplicationDbContext _context;


    public CreatePaymentCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Guid> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        // Получаем график платежа вместе с договором
        var schedule = await _context.PaymentSchedules
            .Include(x => x.Contract)
            .FirstOrDefaultAsync(
                x => x.Id == request.PaymentScheduleId,
                cancellationToken);


        if (schedule is null)
        {
            throw new NotFoundException(
                "Payment schedule not found.");
        }


        // Проверяем, не оплачен ли уже этот месяц
        if (schedule.IsPaid)
        {
            throw new ValidationException(
                "This payment has already been paid.");
        }


        // Проверяем сумму
        if (request.Amount <= 0)
        {
            throw new ValidationException(
                "Payment amount must be greater than zero.");
        }


        var remainingAmount =
            schedule.ExpectedAmount - schedule.PaidAmount;


        if (request.Amount > remainingAmount)
        {
            throw new ValidationException(
                $"Maximum payment amount is {remainingAmount}.");
        }



        // Создаём платеж
        var payment = new Payment
        {
            Id = Guid.NewGuid(),

            ContractId = schedule.ContractId,

            PaymentScheduleId = schedule.Id,

            Amount = request.Amount,

            PaymentDate = DateTime.UtcNow,

            PaymentMethod = request.PaymentMethod,

            Notes = request.Notes
        };


        _context.Payments.Add(payment);



        // Добавляем в навигацию договора
        schedule.Contract.Payments.Add(payment);



        // Обновляем график
        schedule.PaidAmount += request.Amount;



        // Если месяц полностью оплачен
        if (schedule.PaidAmount >= schedule.ExpectedAmount)
        {
            schedule.PaidAmount =
                schedule.ExpectedAmount;

            schedule.IsPaid = true;

            schedule.PaymentDate =
                DateTime.UtcNow;
        }


        schedule.SetUpdated();

        schedule.Contract.SetUpdated();



        // Проверяем все платежи договора
        var allSchedules = await _context.PaymentSchedules
            .Where(x =>
                x.ContractId == schedule.ContractId)
            .ToListAsync(cancellationToken);



        var contractCompleted =
            allSchedules.All(x =>
                x.Id == schedule.Id
                    ? schedule.IsPaid
                    : x.IsPaid);



        if (contractCompleted)
        {
            schedule.Contract.Status =
                ContractStatus.Completed;

            schedule.Contract.SetUpdated();
        }



        await _context.SaveChangesAsync(
            cancellationToken);


        return payment.Id;
    }
}