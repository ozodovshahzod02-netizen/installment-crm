using MediatR;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Commands.UpdateInstallmentContract;

public class UpdateInstallmentContractCommand : IRequest
{
    public Guid Id { get; set; }


    public Guid ProductId { get; set; }


    public decimal DownPayment { get; set; }


    public decimal InterestRate { get; set; }


    public int Months { get; set; }


    public DateTime StartDate { get; set; }


    public string? Notes { get; set; }
}