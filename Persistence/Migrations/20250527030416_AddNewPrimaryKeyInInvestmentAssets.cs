using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPrimaryKeyInInvestmentAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvestmentAssets",
                table: "InvestmentAssets");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "InvestmentAssets",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvestmentAssets",
                table: "InvestmentAssets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentAssets_InvestmentPortfolioId",
                table: "InvestmentAssets",
                column: "InvestmentPortfolioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InvestmentAssets",
                table: "InvestmentAssets");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentAssets_InvestmentPortfolioId",
                table: "InvestmentAssets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "InvestmentAssets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvestmentAssets",
                table: "InvestmentAssets",
                columns: new[] { "InvestmentPortfolioId", "AssetId" });
        }
    }
}
