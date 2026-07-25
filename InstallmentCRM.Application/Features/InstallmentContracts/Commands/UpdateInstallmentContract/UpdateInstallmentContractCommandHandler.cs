using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Commands.UpdateInstallmentContract;

public class UpdateInstallmentContractCommandHandler
    : IRequestHandler<UpdateInstallmentContractCommand>
{
    private readonly IApplicationDbContext _context;


    public UpdateInstallmentContractCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task Handle(
        UpdateInstallmentContractCommand request,
        CancellationToken cancellationToken)
    {
        var contract = await _context.InstallmentContracts
            .Include(x => x.PaymentSchedules)
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);



        if (contract is null)
        {
            throw new NotFoundException(
                "Contract not found.");
        }



        var product = await _context.Products
            .FirstOrDefaultAsync(
                x => x.Id == request.ProductId,
                cancellationToken);



        if (product is null)
        {
            throw new NotFoundException(
                "Product not found.");
        }



        if (request.Months <= 0)
        {
            throw new ValidationException(
                "Months must be greater than zero.");
        }



        if (request.DownPayment < 0)
        {
            throw new ValidationException(
                "Down payment cannot be negative.");
        }



        if (request.DownPayment > product.Price)
        {
            throw new ValidationException(
                "Down payment cannot exceed product price.");
        }



        if (request.InterestRate < 0 ||
            request.InterestRate > 100)
        {
            throw new ValidationException(
                "Interest rate must be between 0 and 100.");
        }



        // расчет новой суммы

        var productPrice = product.Price;


        var remainingAmount =
            productPrice - request.DownPayment;


        var interestAmount =
            remainingAmount *
            request.InterestRate /
            100;


        var totalAmount =
            remainingAmount +
            interestAmount;


        var monthlyPayment =
            Math.Round(
                totalAmount / request.Months,
                2);



        // обновление договора

        contract.ProductId = request.ProductId;

        contract.ProductPrice = productPrice;

        contract.DownPayment = request.DownPayment;

        contract.RemainingAmount = remainingAmount;

        contract.InterestRate = request.InterestRate;

        contract.TotalAmount = totalAmount;

        contract.MonthlyPayment = monthlyPayment;

        contract.Months = request.Months;


        contract.StartDate =
            DateTime.SpecifyKind(
                request.StartDate,
                DateTimeKind.Utc);


        contract.EndDate =
            contract.StartDate
                .AddMonths(request.Months);


        contract.Notes = request.Notes;


        contract.UpdatedAt = DateTime.UtcNow;



        // Проверяем есть ли платежи

        var hasPayments = await _context.Payments
            .AnyAsync(
                x => x.ContractId == contract.Id,
                cancellationToken);



        // если платежей нет - можно пересоздать график

        if (!hasPayments)
        {
            _context.PaymentSchedules.RemoveRange(
                contract.PaymentSchedules);



            for (int month = 1;
                 month <= request.Months;
                 month++)
            {
                decimal amount;


                if (month == request.Months)
                {
                    amount =
                        totalAmount -
                        monthlyPayment *
                        (request.Months - 1);
                }
                else
                {
                    amount = monthlyPayment;
                }



                var schedule =
                    new Domain.Entities.PaymentSchedule
                    {
                        ContractId = contract.Id,

                        MonthNumber = month,

                        ExpectedAmount = amount,

                        PaidAmount = 0,

                        DueDate =
                            contract.StartDate
                                .AddMonths(month),

                        PaymentDate = null,

                        IsPaid = false
                    };


                _context.PaymentSchedules.Add(schedule);
            }
        }



        await _context.SaveChangesAsync(
            cancellationToken);
    }
}