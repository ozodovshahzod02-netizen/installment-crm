using InstallmentCRM.Application.Interfaces;
using MediatR;

namespace InstallmentCRM.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, string>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.RegisterAsync(
            request.FullName,
            request.Email,
            request.Password,
            request.Role);
    }
}
