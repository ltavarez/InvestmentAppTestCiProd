using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnTestPropInAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestProp",
                table: "Assets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestProp",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
