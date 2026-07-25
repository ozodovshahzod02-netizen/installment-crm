namespace InstallmentCRM.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;


    public decimal Price { get; set; }


    public int Quantity { get; set; }


    public string Description { get; set; } = string.Empty;


    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = default!;


    // Договоры рассрочки по этому товару
    public ICollection<InstallmentContract> Contracts { get; set; }
        = new List<InstallmentContract>();
}