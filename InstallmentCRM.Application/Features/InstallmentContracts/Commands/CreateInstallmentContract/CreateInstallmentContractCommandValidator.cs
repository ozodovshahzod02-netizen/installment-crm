using FluentValidation;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Commands.CreateInstallmentContract;

public class CreateInstallmentContractCommandValidator
    : AbstractValidator<CreateInstallmentContractCommand>
{
    public CreateInstallmentContractCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product is required.");

        RuleFor(x => x.DownPayment)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Down payment cannot be negative.");

        RuleFor(x => x.InterestRate)
            .InclusiveBetween(0, 100)
            .WithMessage("Interest rate must be between 0 and 100.");

        RuleFor(x => x.Months)
            .InclusiveBetween(1, 60)
            .WithMessage("Months must be between 1 and 60.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}