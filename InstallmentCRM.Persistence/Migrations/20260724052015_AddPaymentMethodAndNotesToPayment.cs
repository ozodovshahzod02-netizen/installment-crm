using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstallmentCRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodAndNotesToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Payments");
        }
    }
}
