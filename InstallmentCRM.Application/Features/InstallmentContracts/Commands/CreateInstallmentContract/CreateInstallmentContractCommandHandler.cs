using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Commands.CreateInstallmentContract;

public class CreateInstallmentContractCommandHandler
    : IRequestHandler<CreateInstallmentContractCommand, Guid>
{
    private readonly IApplicationDbContext _context;


    public CreateInstallmentContractCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Guid> Handle(
        CreateInstallmentContractCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);


        try
        {
            // Проверяем клиента
            var customer = await _context.Customers
                .FirstOrDefaultAsync(
                    x => x.Id == request.CustomerId,
                    cancellationToken);


            if (customer is null)
            {
                throw new NotFoundException(
                    "Customer not found.");
            }



            // Проверяем товар
            var product = await _context.Products
                .FirstOrDefaultAsync(
                    x => x.Id == request.ProductId,
                    cancellationToken);


            if (product is null)
            {
                throw new NotFoundException(
                    "Product not found.");
            }



            // Проверяем наличие товара
            if (product.Quantity <= 0)
            {
                throw new ValidationException(
                    "Product is out of stock.");
            }



            // Проверяем первоначальный взнос
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



            // Проверяем срок
            if (request.Months <= 0)
            {
                throw new ValidationException(
                    "Months must be greater than zero.");
            }



            // Проверяем процент
            if (request.InterestRate < 0 ||
                request.InterestRate > 100)
            {
                throw new ValidationException(
                    "Interest rate must be between 0 and 100.");
            }



            // Проверяем дату
            if (request.StartDate == default)
            {
                throw new ValidationException(
                    "Start date is required.");
            }



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



            var startDate =
                DateTime.SpecifyKind(
                    request.StartDate,
                    DateTimeKind.Utc);



            var endDate =
                startDate.AddMonths(
                    request.Months);




            // Создаем договор
            var contract = new InstallmentContract
            {
                CustomerId = request.CustomerId,

                ProductId = request.ProductId,


                ProductPrice = productPrice,


                DownPayment = request.DownPayment,


                RemainingAmount = remainingAmount,


                InterestRate = request.InterestRate,


                TotalAmount = totalAmount,


                MonthlyPayment = monthlyPayment,


                Months = request.Months,


                StartDate = startDate,


                EndDate = endDate,


                ContractNumber =
                    $"CTR-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpper()}",


                Notes = request.Notes
            };



            


            _context.InstallmentContracts.Add(contract);



            // уменьшаем склад
            product.Quantity--;

            product.SetUpdated();



            await _context.SaveChangesAsync(
                cancellationToken);




            // Создаем график платежей
            for (int month = 1;
                 month <= contract.Months;
                 month++)
            {

                decimal amount;


                // последний платеж корректируем
                if (month == contract.Months)
                {
                    amount =
                        totalAmount -
                        monthlyPayment *
                        (contract.Months - 1);
                }
                else
                {
                    amount = monthlyPayment;
                }



                var schedule = new PaymentSchedule
                {
                    ContractId = contract.Id,


                    MonthNumber = month,


                    ExpectedAmount = amount,


                    PaidAmount = 0,


                    DueDate =
                        startDate.AddMonths(
                            month),


                    PaymentDate = null,


                    IsPaid = false
                };


                contract.PaymentSchedules.Add(schedule);


                _context.PaymentSchedules.Add(schedule);
            }



            await _context.SaveChangesAsync(
                cancellationToken);



            await transaction.CommitAsync(
                cancellationToken);



            return contract.Id;
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}