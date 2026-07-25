using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Payments.Queries.GetAllPayments;

public record GetAllPaymentsQuery() : IRequest<List<PaymentDto>>;