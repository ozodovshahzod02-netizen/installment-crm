using MediatR;

namespace InstallmentCRM.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name
) : IRequest<Guid>;