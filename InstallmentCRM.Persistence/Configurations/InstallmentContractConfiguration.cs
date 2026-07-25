using InstallmentCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstallmentCRM.Persistence.Configurations;

public class InstallmentContractConfiguration
    : IEntityTypeConfiguration<InstallmentContract>
{
    public void Configure(
        EntityTypeBuilder<InstallmentContract> builder)
    {
        builder.HasKey(x => x.Id);


        builder.Property(x => x.ProductPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        builder.Property(x => x.DownPayment)
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        builder.Property(x => x.RemainingAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        builder.Property(x => x.InterestRate)
            .HasColumnType("decimal(5,2)")
            .IsRequired();


        builder.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        builder.Property(x => x.MonthlyPayment)
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        builder.Property(x => x.ContractNumber)
            .HasMaxLength(50)
            .IsRequired();


        builder.HasIndex(x => x.ContractNumber)
            .IsUnique();


        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Contracts)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.Product)
            .WithMany(x => x.Contracts)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Contract)
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(x => x.PaymentSchedules)
            .WithOne(x => x.Contract)
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}