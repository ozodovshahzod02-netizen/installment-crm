namespace InstallmentCRM.Application.DTOs;

public class UpcomingPaymentDto
{
    public Guid ScheduleId { get; set; }

    public Guid ContractId { get; set; }

    public string ContractNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public int DaysLeft { get; set; }
}