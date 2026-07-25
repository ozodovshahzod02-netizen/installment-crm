using InstallmentCRM.Application.Features.PaymentSchedules.Queries.GetPaymentSchedulesByContract;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstallmentCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentSchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentSchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{contractId:guid}")]
    public async Task<IActionResult> Get(Guid contractId)
    {
        var result = await _mediator.Send(
            new GetPaymentSchedulesByContractQuery(contractId));

        return Ok(result);
    }
}