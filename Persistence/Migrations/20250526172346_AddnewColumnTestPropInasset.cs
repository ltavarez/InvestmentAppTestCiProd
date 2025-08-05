using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddnewColumnTestPropInasset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestProp",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestProp",
                table: "Assets");
        }
    }
}
