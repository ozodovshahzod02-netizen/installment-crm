using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Dashboard.Queries.GetTopDebtors;

public record GetTopDebtorsQuery(int Count = 10)
    : IRequest<List<TopDebtorDto>>;