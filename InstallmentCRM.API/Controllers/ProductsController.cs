using InstallmentCRM.Application.Features.Products.Commands.CreateProduct;
using InstallmentCRM.Application.Features.Products.Commands.DeleteProduct;
using InstallmentCRM.Application.Features.Products.Commands.UpdateProduct;
using InstallmentCRM.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstallmentCRM.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager,Cashier")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;


    public ProductsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }



    [Authorize(Roles = "Manager")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            command,
            cancellationToken);


        return Ok(new
        {
            Id = id,
            Message = "Товар успешно создан."
        });
    }




    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(
            new GetAllProductsQuery(),
            cancellationToken);


        return Ok(products);
    }




    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(
            new GetProductByIdQuery(id),
            cancellationToken);


        if (product == null)
        {
            return NotFound(new
            {
                Message = "Товар не найден."
            });
        }


        return Ok(product);
    }





    [Authorize(Roles = "Manager")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductCommand command,
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


        return Ok(new
        {
            Message = "Товар успешно обновлен."
        });
    }





    [Authorize(Roles = "Manager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DeleteProductCommand(id),
            cancellationToken);


        return Ok(new
        {
            Message = "Товар успешно удален."
        });
    }
}