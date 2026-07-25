namespace InstallmentCRM.Application.Interfaces;

public interface IIdentityService
{
    Task<string> RegisterAsync(
        string fullName,
        string email,
        string password,
        string? role);

    Task<string> LoginAsync(
        string email,
        string password);
}
