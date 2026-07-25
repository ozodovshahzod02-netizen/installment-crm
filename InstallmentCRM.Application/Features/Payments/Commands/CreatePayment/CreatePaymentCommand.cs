using InstallmentCRM.Domain.Enums;
using MediatR;

namespace InstallmentCRM.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommand : IRequest<Guid>
{
    public Guid PaymentScheduleId { get; set; }


    public decimal Amount { get; set; }


    public PaymentMethod PaymentMethod { get; set; }


    public string? Notes { get; set; }
}