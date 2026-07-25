using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Queries.GetInstallmentContractById;

public record GetInstallmentContractByIdQuery(Guid Id)
    : IRequest<InstallmentContractDto?>;