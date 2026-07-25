using InstallmentCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InstallmentCRM.Persistence.Configurations;

public class PaymentConfiguration
    : IEntityTypeConfiguration<Payment>
{
    public void Configure(
        EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);


        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();


        builder.Property(x => x.PaymentDate)
            .IsRequired();


        builder.Property(x => x.PaymentMethod)
            .IsRequired();


        builder.Property(x => x.Notes)
            .HasMaxLength(500);



        builder.HasOne(x => x.Contract)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.HasOne(x => x.PaymentSchedule)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.PaymentScheduleId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.HasIndex(x => x.ContractId);

        builder.HasIndex(x => x.PaymentScheduleId);
    }
}