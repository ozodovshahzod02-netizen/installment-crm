using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstallmentCRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "PaymentSchedules",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "PaymentSchedules",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "PaymentSchedules");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "PaymentSchedules");
        }
    }
}
