using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Domain.Entities;
using MediatR;

namespace InstallmentCRM.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = new Category
        {
         
            Name = request.Name
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}