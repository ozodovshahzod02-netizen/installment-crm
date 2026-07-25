using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstallmentCRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInstallmentContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "InstallmentContracts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DownPayment",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "InstallmentContracts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPayment",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "InstallmentContracts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductPrice",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingAmount",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "InstallmentContracts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "InstallmentContracts");

            migrationBuilder.DropColumn(
                name: "DownPayment",
                table: "InstallmentContracts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "InstallmentContracts");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "InstallmentContracts");

            migrationBuilder.DropColumn(
                name: "MonthlyPayment",
                table: "InstallmentContracts");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "InstallmentContracts");

            migrationBuilder.DropColumn(
                name: "ProductPrice",
                table: "InstallmentContracts");

            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                table: "InstallmentContracts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InstallmentContracts");
        }
    }
}
