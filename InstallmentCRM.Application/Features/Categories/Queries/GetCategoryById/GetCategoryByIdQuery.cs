using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Categories.Queries;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;