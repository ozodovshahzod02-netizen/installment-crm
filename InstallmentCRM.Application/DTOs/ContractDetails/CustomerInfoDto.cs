namespace InstallmentCRM.Application.DTOs.ContractDetails;

public class CustomerInfoDto
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}