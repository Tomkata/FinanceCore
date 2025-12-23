using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Transactions",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true,
                oldDefaultValue: new byte[] { 0 });

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "TransactionEntries",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true,
                oldDefaultValue: new byte[] { 0 });

            migrationBuilder.AddColumn<int>(
                name: "LedgerAccountType",
                table: "TransactionEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Customers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true,
                oldDefaultValue: new byte[] { 0 });

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Accounts",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true,
                oldDefaultValue: new byte[] { 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LedgerAccountType",
                table: "TransactionEntries");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Transactions",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { 0 },
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "TransactionEntries",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { 0 },
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Customers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { 0 },
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "Accounts",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[] { 0 },
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);
        }
    }
}
