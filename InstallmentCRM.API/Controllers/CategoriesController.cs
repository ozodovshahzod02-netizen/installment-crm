using InstallmentCRM.Application.Features.Categories.Commands.CreateCategory;
using InstallmentCRM.Application.Features.Categories.Commands.DeleteCategory;
using InstallmentCRM.Application.Features.Categories.Commands.UpdateCategory;
using InstallmentCRM.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstallmentCRM.API.Controllers;

[Authorize(Roles = "Manager")]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Создать категорию
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var id = await _mediator.Send(command);

        return Ok(new
        {
            Id = id,
            Message = "Категория успешно создана."
        });
    }

    /// <summary>
    /// Получить список категорий
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());

        return Ok(categories);
    }

    /// <summary>
    /// Получить категорию по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (category == null)
        {
            return NotFound(new
            {
                Message = "Категория не найдена."
            });
        }

        return Ok(category);
    }

    /// <summary>
    /// Обновить категорию
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                Message = "Id в URL не совпадает с Id в теле запроса."
            });
        }

        await _mediator.Send(command);

        return Ok(new
        {
            Message = "Категория успешно обновлена."
        });
    }

    /// <summary>
    /// Удалить категорию
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));

        return Ok(new
        {
            Message = "Категория успешно удалена."
        });
    }
}