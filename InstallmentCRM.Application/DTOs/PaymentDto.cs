using InstallmentCRM.Domain.Enums;

namespace InstallmentCRM.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }

    public Guid ContractId { get; set; }

    public Guid PaymentScheduleId { get; set; }

    public string ContractNumber { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }
}