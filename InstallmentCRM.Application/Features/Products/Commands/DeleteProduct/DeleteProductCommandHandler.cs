using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);

        if (product is null)
        {
            throw new NotFoundException("Product not found.");
        }

        var hasContracts = await _context.InstallmentContracts
            .AnyAsync(c => c.ProductId == product.Id, cancellationToken);

        if (hasContracts)
        {
            throw new ValidationException(
                "Cannot delete product because it is used in installment contracts.");
        }

        _context.Products.Remove(product);

        await _context.SaveChangesAsync(cancellationToken);
    }
}