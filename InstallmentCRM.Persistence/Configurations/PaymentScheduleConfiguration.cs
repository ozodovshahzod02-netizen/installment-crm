using InstallmentCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstallmentCRM.Persistence.Configurations;

public class PaymentScheduleConfiguration
    : IEntityTypeConfiguration<PaymentSchedule>
{
    public void Configure(
        EntityTypeBuilder<PaymentSchedule> builder)
    {
        builder.HasKey(x => x.Id);


        builder.Property(x => x.MonthNumber)
            .IsRequired();


        builder.Property(x => x.ExpectedAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        builder.Property(x => x.PaidAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        builder.Property(x => x.DueDate)
            .IsRequired();


        builder.HasOne(x => x.Contract)
            .WithMany(x => x.PaymentSchedules)
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(x => x.Payments)
            .WithOne(x => x.PaymentSchedule)
            .HasForeignKey(x => x.PaymentScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}