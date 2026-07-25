using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id,
                cancellationToken);

        if (product is null)
            throw new NotFoundException("Product not found.");

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                c => c.Id == request.CategoryId,
                cancellationToken);

        if (category is null)
            throw new NotFoundException("Category not found.");

        if (request.Price <= 0)
            throw new ValidationException("Price must be greater than zero.");

        if (request.Quantity < 0)
            throw new ValidationException("Quantity cannot be negative.");

        product.Name = request.Name;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;

        product.SetUpdated();

        await _context.SaveChangesAsync(cancellationToken);
    }
}