using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeCategoryForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialRecords_Categories_CategoryId",
                table: "FinancialRecords");

            migrationBuilder.DropIndex(
                name: "IX_FinancialRecords_CategoryId",
                table: "FinancialRecords");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categories_Id_RecordTypeId",
                table: "Categories",
                columns: new[] { "Id", "RecordTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialRecords_CategoryId_RecordTypeId",
                table: "FinancialRecords",
                columns: new[] { "CategoryId", "RecordTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialRecords_Categories_CategoryId_RecordTypeId",
                table: "FinancialRecords",
                columns: new[] { "CategoryId", "RecordTypeId" },
                principalTable: "Categories",
                principalColumns: new[] { "Id", "RecordTypeId" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialRecords_Categories_CategoryId_RecordTypeId",
                table: "FinancialRecords");

            migrationBuilder.DropIndex(
                name: "IX_FinancialRecords_CategoryId_RecordTypeId",
                table: "FinancialRecords");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categories_Id_RecordTypeId",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialRecords_CategoryId",
                table: "FinancialRecords",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialRecords_Categories_CategoryId",
                table: "FinancialRecords",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
