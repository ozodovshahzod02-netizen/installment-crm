namespace InstallmentCRM.Application.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string PassportNumber { get; set; } = string.Empty;
}