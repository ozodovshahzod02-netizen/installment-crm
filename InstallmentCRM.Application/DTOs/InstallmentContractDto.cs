namespace InstallmentCRM.Application.DTOs;

public class InstallmentContractDto
{
    public Guid Id { get; set; }

    public string ContractNumber { get; set; } = string.Empty;


    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;


    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;


    public decimal ProductPrice { get; set; }

    public decimal DownPayment { get; set; }

    public decimal RemainingAmount { get; set; }

    public decimal InterestRate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal MonthlyPayment { get; set; }


    public int Months { get; set; }


    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }


    public string Status { get; set; } = string.Empty;


    public string? Notes { get; set; }


    // График платежей
    public List<PaymentScheduleDto> PaymentSchedules { get; set; }
        = new();
}