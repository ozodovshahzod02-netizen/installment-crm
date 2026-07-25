using InstallmentCRM.Application.DTOs;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Queries.GetAllInstallmentContracts;

public class GetAllInstallmentContractsQueryHandler
    : IRequestHandler<GetAllInstallmentContractsQuery, List<InstallmentContractDto>>
{
    private readonly IApplicationDbContext _context;


    public GetAllInstallmentContractsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<List<InstallmentContractDto>> Handle(
        GetAllInstallmentContractsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.InstallmentContracts

            .Include(x => x.Customer)

            .Include(x => x.Product)

            .OrderByDescending(x => x.CreatedAt)

            .Select(x => new InstallmentContractDto
            {
                Id = x.Id,


                ContractNumber = x.ContractNumber,


                CustomerId = x.CustomerId,

                CustomerName =
                    x.Customer.FirstName + " " +
                    x.Customer.LastName,


                ProductId = x.ProductId,

                ProductName =
                    x.Product.Name,


                ProductPrice = x.ProductPrice,


                DownPayment = x.DownPayment,


                RemainingAmount = x.RemainingAmount,


                InterestRate = x.InterestRate,


                TotalAmount = x.TotalAmount,


                MonthlyPayment = x.MonthlyPayment,


                Months = x.Months,


                StartDate = x.StartDate,


                EndDate = x.EndDate,


                Status = x.Status.ToString(),


                Notes = x.Notes
            })

            .ToListAsync(cancellationToken);
    }
}