using MediatR;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Commands.DeleteInstallmentContract;

public record DeleteInstallmentContractCommand(Guid Id) : IRequest;