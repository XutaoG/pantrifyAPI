using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pantrifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class QuantitySplit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "RecipeIngredient",
                newName: "QuantityWhole");

            migrationBuilder.AddColumn<string>(
                name: "QuantityFraction",
                table: "RecipeIngredient",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityFraction",
                table: "RecipeIngredient");

            migrationBuilder.RenameColumn(
                name: "QuantityWhole",
                table: "RecipeIngredient",
                newName: "Quantity");
        }
    }
}
