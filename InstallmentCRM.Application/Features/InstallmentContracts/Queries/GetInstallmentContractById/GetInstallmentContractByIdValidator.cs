using FluentValidation;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Queries.GetInstallmentContractById;

public class GetInstallmentContractByIdValidator
    : AbstractValidator<GetInstallmentContractByIdQuery>
{
    public GetInstallmentContractByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Contract Id is required.");
    }
}