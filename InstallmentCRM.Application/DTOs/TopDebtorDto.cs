namespace InstallmentCRM.Application.DTOs;

public class TopDebtorDto
{
    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string ContractNumber { get; set; } = string.Empty;

    public decimal RemainingDebt { get; set; }

    public int OverduePayments { get; set; }

    public int DaysLate { get; set; }
}