using InstallmentCRM.Application.Features.Payments.Commands.CreatePayment;
using InstallmentCRM.Application.Features.Payments.Commands.DeletePayment;
using InstallmentCRM.Application.Features.Payments.Commands.UpdatePayment;
using InstallmentCRM.Application.Features.Payments.Queries.GetAllPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstallmentCRM.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager,Cashier")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;


    public PaymentsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }



    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePaymentCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            command,
            cancellationToken);


        return Ok(new
        {
            PaymentId = id,

            Message = "Платеж успешно создан."
        });
    }




    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var payments = await _mediator.Send(
            new GetAllPaymentsQuery(),
            cancellationToken);


        return Ok(payments);
    }



    /// <summary>
    /// Изменить платеж (сумму, способ оплаты, примечание).
    /// Доступно только менеджеру, так как влияет на финансовый график.
    /// </summary>
    [Authorize(Roles = "Manager")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePaymentCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                Message = "Id в URL не совпадает с Id в теле запроса."
            });
        }

        await _mediator.Send(command, cancellationToken);

        return Ok(new
        {
            Message = "Платеж успешно обновлен."
        });
    }



    /// <summary>
    /// Удалить платеж (откатывает график платежей).
    /// Доступно только менеджеру.
    /// </summary>
    [Authorize(Roles = "Manager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeletePaymentCommand(id), cancellationToken);

        return Ok(new
        {
            Message = "Платеж успешно удален."
        });
    }
}
