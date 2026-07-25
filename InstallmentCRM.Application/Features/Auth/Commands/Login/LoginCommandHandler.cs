using InstallmentCRM.Application.Interfaces;
using MediatR;

namespace InstallmentCRM.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, string>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(
            request.Email,
            request.Password);
    }
}