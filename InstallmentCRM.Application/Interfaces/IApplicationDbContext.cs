using InstallmentCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InstallmentCRM.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }

    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    DbSet<InstallmentContract> InstallmentContracts { get; }

    DbSet<Payment> Payments { get; }

    DbSet<PaymentSchedule> PaymentSchedules { get; }


    DatabaseFacade Database { get; }


    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}