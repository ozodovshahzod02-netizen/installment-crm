using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == request.CategoryId,
                cancellationToken);

        if (category == null)
            throw new NotFoundException("Category not found.");

        if (request.Price <= 0)
            throw new ValidationException("Price must be greater than zero.");

        if (request.Quantity < 0)
            throw new ValidationException("Quantity cannot be negative.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Quantity = request.Quantity,
            Description = request.Description,
            CategoryId = request.CategoryId
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}