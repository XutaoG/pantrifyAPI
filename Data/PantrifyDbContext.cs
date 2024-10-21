using Microsoft.EntityFrameworkCore;
using Pantrify.API.Model;

namespace Pantrify.API.Data
{
	public class PantrifyDbContext : DbContext
	{
		public DbSet<Recipe> Recipes { get; set; }

		public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

		public DbSet<RecipeInstruction> RecipeInstructions { get; set; }

		public DbSet<RecipeImage> RecipeImages { get; set; }

		public DbSet<Ingredient> Ingredients { get; set; }

		public PantrifyDbContext(DbContextOptions<PantrifyDbContext> options) : base(options) { }
	}
}