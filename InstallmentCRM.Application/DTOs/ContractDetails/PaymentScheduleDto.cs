namespace InstallmentCRM.Application.DTOs.ContractDetails;

public class PaymentScheduleDto
{
    public Guid Id { get; set; }

    public int MonthNumber { get; set; }

    public decimal ExpectedAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public bool IsPaid { get; set; }

    public bool IsOverdue { get; set; }

    public int DaysLate { get; set; }
}