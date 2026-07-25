using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Category not found.");
        }

        var hasProducts = await _context.Products
            .AnyAsync(p => p.CategoryId == category.Id, cancellationToken);

        if (hasProducts)
        {
            throw new ValidationException(
                "Cannot delete category because it still contains products. Move or delete the products first.");
        }

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
