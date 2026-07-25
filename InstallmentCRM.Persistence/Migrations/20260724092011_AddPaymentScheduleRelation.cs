using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstallmentCRM.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentScheduleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Payments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentScheduleId",
                table: "Payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentScheduleId",
                table: "Payments",
                column: "PaymentScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentSchedules_PaymentScheduleId",
                table: "Payments",
                column: "PaymentScheduleId",
                principalTable: "PaymentSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentSchedules_PaymentScheduleId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentScheduleId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentScheduleId",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Payments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");
        }
    }
}
