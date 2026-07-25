using MediatR;

namespace InstallmentCRM.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
}