using InstallmentCRM.Application.Features.InstallmentContracts.Commands.CreateInstallmentContract;
using InstallmentCRM.Application.Features.InstallmentContracts.Commands.DeleteInstallmentContract;
using InstallmentCRM.Application.Features.InstallmentContracts.Commands.UpdateInstallmentContract;
using InstallmentCRM.Application.Features.InstallmentContracts.Queries.GetAllInstallmentContracts;
using InstallmentCRM.Application.Features.InstallmentContracts.Queries.GetInstallmentContractById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstallmentCRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager,Cashier")]
public class InstallmentContractsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InstallmentContractsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Создать договор рассрочки
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInstallmentContractCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);

        return Ok(new
        {
            Id = id,
            Message = "Договор рассрочки успешно создан."
        });
    }

    /// <summary>
    /// Получить список договоров рассрочки
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var contracts = await _mediator.Send(
            new GetAllInstallmentContractsQuery(),
            cancellationToken);

        return Ok(contracts);
    }

    /// <summary>
    /// Получить договор рассрочки по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var contract = await _mediator.Send(
            new GetInstallmentContractByIdQuery(id),
            cancellationToken);

        if (contract is null)
        {
            return NotFound(new
            {
                Message = "Договор не найден."
            });
        }

        return Ok(contract);
    }

    /// <summary>
    /// Обновить договор рассрочки
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateInstallmentContractCommand command,
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
            Message = "Договор успешно обновлен."
        });
    }

    /// <summary>
    /// Удалить договор рассрочки
    /// </summary>
    [Authorize(Roles = "Manager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteInstallmentContractCommand(id), cancellationToken);

        return Ok(new
        {
            Message = "Договор успешно удален."
        });
    }
}
