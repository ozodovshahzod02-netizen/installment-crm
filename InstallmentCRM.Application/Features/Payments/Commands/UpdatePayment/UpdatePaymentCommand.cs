using InstallmentCRM.Domain.Enums;
using MediatR;

namespace InstallmentCRM.Application.Features.Payments.Commands.UpdatePayment;

public class UpdatePaymentCommand : IRequest
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }
}