using InstallmentCRM.Application.Features.Customers.Commands.CreateCustomer;
using InstallmentCRM.Application.Features.Customers.Commands.DeleteCustomer;
using InstallmentCRM.Application.Features.Customers.Commands.UpdateCustomer;
using InstallmentCRM.Application.Features.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstallmentCRM.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager,Cashier")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;


    public CustomersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }



    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            command,
            cancellationToken);


        return Ok(new
        {
            Id = id,

            Message = "Клиент успешно создан."
        });
    }



    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var customers = await _mediator.Send(
            new GetAllCustomersQuery(),
            cancellationToken);


        return Ok(customers);
    }



    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _mediator.Send(
            new GetCustomerByIdQuery(id),
            cancellationToken);


        if (customer is null)
        {
            return NotFound(new
            {
                Message = "Клиент не найден."
            });
        }


        return Ok(customer);
    }




    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                Message =
                "Id в URL не совпадает с Id в теле запроса."
            });
        }


        await _mediator.Send(
            command,
            cancellationToken);


        return NoContent();
    }





    [Authorize(Roles = "Manager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DeleteCustomerCommand(id),
            cancellationToken);


        return NoContent();
    }
}