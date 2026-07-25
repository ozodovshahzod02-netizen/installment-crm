namespace InstallmentCRM.Application.DTOs;

public class DashboardDto
{
    public int TotalCustomers { get; set; }

    public int TotalProducts { get; set; }

    public int TotalContracts { get; set; }

    public int ActiveContracts { get; set; }

    public int CompletedContracts { get; set; }

    public decimal TotalContractAmount { get; set; }

    public decimal TotalPaidAmount { get; set; }

    public decimal RemainingAmount { get; set; }

    public int PaidSchedules { get; set; }

    public int OverdueSchedules { get; set; }

    public int DueTodaySchedules { get; set; }
}