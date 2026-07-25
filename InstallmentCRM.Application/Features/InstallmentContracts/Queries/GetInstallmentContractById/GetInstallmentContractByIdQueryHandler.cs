using InstallmentCRM.Application.DTOs;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Queries.GetInstallmentContractById;

public class GetInstallmentContractByIdQueryHandler
    : IRequestHandler<GetInstallmentContractByIdQuery, InstallmentContractDto?>
{
    private readonly IApplicationDbContext _context;

    public GetInstallmentContractByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<InstallmentContractDto?> Handle(
        GetInstallmentContractByIdQuery request,
        CancellationToken cancellationToken)
    {
        var contract = await _context.InstallmentContracts
            .Include(c => c.Customer)
            .Include(c => c.Product)
            .Include(c => c.PaymentSchedules)
            .FirstOrDefaultAsync(
                c => c.Id == request.Id,
                cancellationToken);


        if (contract is null)
        {
            return null;
        }


        return new InstallmentContractDto
        {
            Id = contract.Id,

            ContractNumber = contract.ContractNumber,


            CustomerId = contract.CustomerId,

            CustomerName =
                $"{contract.Customer.FirstName} {contract.Customer.LastName}",


            ProductId = contract.ProductId,

            ProductName = contract.Product.Name,


            ProductPrice = contract.ProductPrice,

            DownPayment = contract.DownPayment,

            RemainingAmount = contract.RemainingAmount,

            InterestRate = contract.InterestRate,

            TotalAmount = contract.TotalAmount,

            MonthlyPayment = contract.MonthlyPayment,


            Months = contract.Months,


            StartDate = contract.StartDate,

            EndDate = contract.EndDate,


            Status = contract.Status.ToString(),


            Notes = contract.Notes,


            PaymentSchedules = contract.PaymentSchedules
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
                .ToList()
        };
    }
}