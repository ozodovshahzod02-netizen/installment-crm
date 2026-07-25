using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstallmentCRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInstallmentContractConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentContracts_Customers_CustomerId",
                table: "InstallmentContracts");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentContracts_Products_ProductId",
                table: "InstallmentContracts");

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAmount",
                table: "PaymentSchedules",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedAmount",
                table: "PaymentSchedules",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "InstallmentContracts",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "RemainingAmount",
                table: "InstallmentContracts",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProductPrice",
                table: "InstallmentContracts",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyPayment",
                table: "InstallmentContracts",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "InstallmentContracts",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "DownPayment",
                table: "InstallmentContracts",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                table: "InstallmentContracts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentContracts_ContractNumber",
                table: "InstallmentContracts",
                column: "ContractNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentContracts_Customers_CustomerId",
                table: "InstallmentContracts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentContracts_Products_ProductId",
                table: "InstallmentContracts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentContracts_Customers_CustomerId",
                table: "InstallmentContracts");

            migrationBuilder.DropForeignKey(
                name: "FK_InstallmentContracts_Products_ProductId",
                table: "InstallmentContracts");

            migrationBuilder.DropIndex(
                name: "IX_InstallmentContracts_ContractNumber",
                table: "InstallmentContracts");

            migrationBuilder.AlterColumn<decimal>(
                name: "PaidAmount",
                table: "PaymentSchedules",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpectedAmount",
                table: "PaymentSchedules",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RemainingAmount",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProductPrice",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyPayment",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DownPayment",
                table: "InstallmentContracts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                table: "InstallmentContracts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentContracts_Customers_CustomerId",
                table: "InstallmentContracts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstallmentContracts_Products_ProductId",
                table: "InstallmentContracts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
