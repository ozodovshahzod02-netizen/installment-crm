using InstallmentCRM.Application.DTOs;
using MediatR;

namespace InstallmentCRM.Application.Features.Customers.Queries;

public record GetAllCustomersQuery : IRequest<List<CustomerDto>>;