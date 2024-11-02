using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pantrifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderToImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "RecipeImage",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "RecipeImage");
        }
    }
}
