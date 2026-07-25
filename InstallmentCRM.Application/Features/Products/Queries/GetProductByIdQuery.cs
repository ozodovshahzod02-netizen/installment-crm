using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;