using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Queries.GetAllInstallmentContracts;

public record GetAllInstallmentContractsQuery
    : IRequest<List<InstallmentContractDto>>;