using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pantrifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToRecipeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Recipe",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Recipe");
        }
    }
}
