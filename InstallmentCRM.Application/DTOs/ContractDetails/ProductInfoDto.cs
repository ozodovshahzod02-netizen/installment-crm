namespace InstallmentCRM.Application.DTOs.ContractDetails;

public class ProductInfoDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
}