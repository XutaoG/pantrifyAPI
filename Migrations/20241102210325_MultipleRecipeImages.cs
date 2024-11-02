using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pantrifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class MultipleRecipeImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecipeImage_RecipeId",
                table: "RecipeImage");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeImage_RecipeId",
                table: "RecipeImage",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecipeImage_RecipeId",
                table: "RecipeImage");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeImage_RecipeId",
                table: "RecipeImage",
                column: "RecipeId",
                unique: true);
        }
    }
}
