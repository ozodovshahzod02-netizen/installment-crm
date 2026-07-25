using InstallmentCRM.Application.Common.Exceptions;
using InstallmentCRM.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Application.Features.InstallmentContracts.Commands.DeleteInstallmentContract;

public class DeleteInstallmentContractCommandHandler
    : IRequestHandler<DeleteInstallmentContractCommand>
{
    private readonly IApplicationDbContext _context;


    public DeleteInstallmentContractCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task Handle(
        DeleteInstallmentContractCommand request,
        CancellationToken cancellationToken)
    {
        var contract = await _context.InstallmentContracts
            .Include(x => x.PaymentSchedules)
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);



        if (contract is null)
        {
            throw new NotFoundException(
                "Contract not found.");
        }



        // Проверяем платежи

        var hasPayments = await _context.Payments
            .AnyAsync(
                x => x.ContractId == contract.Id,
                cancellationToken);



        if (hasPayments)
        {
            throw new ValidationException(
                "Cannot delete contract because payments exist.");
        }



        // Возвращаем товар на склад

        var product = await _context.Products
            .FirstOrDefaultAsync(
                x => x.Id == contract.ProductId,
                cancellationToken);



        if (product != null)
        {
            product.Quantity++;

            product.UpdatedAt = DateTime.UtcNow;
        }



        // Удаляем график платежей

        _context.PaymentSchedules.RemoveRange(
            contract.PaymentSchedules);



        // Удаляем договор

        _context.InstallmentContracts.Remove(
            contract);



        await _context.SaveChangesAsync(
            cancellationToken);
    }
}