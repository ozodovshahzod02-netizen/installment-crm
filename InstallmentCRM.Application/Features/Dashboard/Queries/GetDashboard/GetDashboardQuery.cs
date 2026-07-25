using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Dashboard.Queries.GetDashboard;

public record GetDashboardQuery : IRequest<DashboardDto>;