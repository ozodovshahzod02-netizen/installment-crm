namespace InstallmentCRM.Domain.Entities;

public class PaymentSchedule : BaseEntity
{
    public Guid ContractId { get; set; }

    public InstallmentContract Contract { get; set; } = default!;


    public int MonthNumber { get; set; }


    public decimal ExpectedAmount { get; set; }


    public decimal PaidAmount { get; set; }


    public DateTime DueDate { get; set; }


    public DateTime? PaymentDate { get; set; }


    public bool IsPaid { get; set; } = false;


    public ICollection<Payment> Payments { get; set; }
        = new List<Payment>();
}