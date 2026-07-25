using FluentValidation;

namespace InstallmentCRM.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator
    : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{9,15}$")
            .WithMessage("Phone number is invalid.");

        RuleFor(x => x.PassportNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}