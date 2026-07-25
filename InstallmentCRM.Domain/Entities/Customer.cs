namespace InstallmentCRM.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;

    public ICollection<InstallmentContract> Contracts { get; set; } = new List<InstallmentContract>();
}