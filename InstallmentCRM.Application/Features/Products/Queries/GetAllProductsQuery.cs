using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Products.Queries;

public record GetAllProductsQuery : IRequest<List<ProductDto>>;