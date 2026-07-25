using InstallmentCRM.Application.Features.Dashboard.Queries.GetDashboard;
using InstallmentCRM.Application.Features.Dashboard.Queries.GetTopDebtors;
using InstallmentCRM.Application.Features.Dashboard.Queries.GetUpcomingPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstallmentCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var dashboard = await _mediator.Send(new GetDashboardQuery());

        return Ok(dashboard);
    }

    [HttpGet("top-debtors")]
    public async Task<IActionResult> GetTopDebtors()
    {
        var debtors = await _mediator.Send(new GetTopDebtorsQuery());

        return Ok(debtors);
    }

    [HttpGet("upcoming-payments")]
    public async Task<IActionResult> GetUpcomingPayments()
    {
        var payments = await _mediator.Send(new GetUpcomingPaymentsQuery());

        return Ok(payments);
    }
}