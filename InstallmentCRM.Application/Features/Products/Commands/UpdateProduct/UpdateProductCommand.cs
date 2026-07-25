using MediatR;

namespace InstallmentCRM.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string Description { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
}