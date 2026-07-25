using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Categories.Queries;

public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;