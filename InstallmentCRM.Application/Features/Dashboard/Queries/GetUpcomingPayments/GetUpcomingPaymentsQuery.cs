using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Dashboard.Queries.GetUpcomingPayments;

public record GetUpcomingPaymentsQuery(int Days = 7)
    : IRequest<List<UpcomingPaymentDto>>;