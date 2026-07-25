using InstallmentCRM.Application.Interfaces;
using InstallmentCRM.Domain.Entities;
using InstallmentCRM.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstallmentCRM.Persistence.Context;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>,
      IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<InstallmentContract> InstallmentContracts => Set<InstallmentContract>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<PaymentSchedule> PaymentSchedules => Set<PaymentSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}