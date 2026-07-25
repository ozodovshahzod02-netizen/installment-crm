using MediatR;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Commands.CreateInstallmentContract;

public class CreateInstallmentContractCommand : IRequest<Guid>
{
    public Guid CustomerId { get; set; }


    public Guid ProductId { get; set; }


    public decimal DownPayment { get; set; }


    public decimal InterestRate { get; set; }


    public int Months { get; set; }


    public DateTime StartDate { get; set; }


    public string? Notes { get; set; }
}