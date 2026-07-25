using InstallmentCRM.Domain.Enums;

namespace InstallmentCRM.Application.DTOs.ContractDetails;

public class ContractDetailsDto
{
    public Guid Id { get; set; }

    public string ContractNumber { get; set; } = string.Empty;

    public ContractStatus Status { get; set; }

    public CustomerInfoDto Customer { get; set; } = default!;

    public ProductInfoDto Product { get; set; } = default!;

    public decimal ProductPrice { get; set; }

    public decimal DownPayment { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public decimal ProgressPercent { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<PaymentScheduleDto> PaymentSchedules { get; set; } = [];

    public List<PaymentHistoryDto> Payments { get; set; } = [];
}