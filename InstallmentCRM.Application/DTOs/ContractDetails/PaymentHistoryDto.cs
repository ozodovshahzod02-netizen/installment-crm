using InstallmentCRM.Domain.Enums;

namespace InstallmentCRM.Application.DTOs.ContractDetails;

public class PaymentHistoryDto
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }
}