using MediatR;

namespace InstallmentCRM.Application.Features.Payments.Commands.DeletePayment;

public record DeletePaymentCommand(Guid Id) : IRequest;