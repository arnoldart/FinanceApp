using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.API.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeDashboardReadIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_DeletedAt_CreatedAt",
                table: "Transactions",
                columns: new[] { "UserId", "DeletedAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_Type_DeletedAt_CreatedAt",
                table: "Transactions",
                columns: new[] { "UserId", "Type", "DeletedAt", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId_DeletedAt_CreatedAt",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId_Type_DeletedAt_CreatedAt",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");
        }
    }
}
