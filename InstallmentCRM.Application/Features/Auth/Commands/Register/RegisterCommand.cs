using MediatR;

namespace InstallmentCRM.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<string>
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Manager, Seller или Cashier. Если не указано - по умолчанию Seller.
    /// </summary>
    public string? Role { get; set; }
}
