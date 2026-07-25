using InstallmentCRM.Domain.Enums;

namespace InstallmentCRM.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid ContractId { get; set; }
    public InstallmentContract Contract { get; set; } = default!;

    public Guid PaymentScheduleId { get; set; }
    public PaymentSchedule PaymentSchedule { get; set; } = default!;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }
}