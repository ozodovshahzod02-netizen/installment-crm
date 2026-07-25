using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.PaymentSchedules.Queries.GetPaymentSchedulesByContract;

public record GetPaymentSchedulesByContractQuery(Guid ContractId)
    : IRequest<List<PaymentScheduleDto>>;