using InstallmentCRM.Domain.Enums;

namespace InstallmentCRM.Domain.Entities;

public class InstallmentContract : BaseEntity
{
    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = default!;


    public Guid ProductId { get; set; }

    public Product Product { get; set; } = default!;


    public decimal ProductPrice { get; set; }


    public decimal DownPayment { get; set; }


    public decimal RemainingAmount { get; set; }


    public decimal InterestRate { get; set; }


    public decimal TotalAmount { get; set; }


    public decimal MonthlyPayment { get; set; }


    public int Months { get; set; }


    public DateTime StartDate { get; set; }


    public DateTime EndDate { get; set; }


    public string ContractNumber { get; set; } = string.Empty;


    public ContractStatus Status { get; set; }
        = ContractStatus.Active;


    public string? Notes { get; set; }


    // История всех оплат по договору
    public ICollection<Payment> Payments { get; set; }
        = new List<Payment>();


    // График платежей
    public ICollection<PaymentSchedule> PaymentSchedules { get; set; }
        = new List<PaymentSchedule>();
}