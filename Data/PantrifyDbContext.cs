using Microsoft.EntityFrameworkCore;
using Pantrify.API.Models;

namespace Pantrify.API.Data
{
	public class PantrifyDbContext : DbContext
	{
		public DbSet<Recipe> Recipes { get; set; }

		public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

		public DbSet<RecipeInstruction> RecipeInstructions { get; set; }

		public DbSet<RecipeImage> RecipeImages { get; set; }

		public DbSet<Ingredient> Ingredients { get; set; }

		// Auth
		public DbSet<User> Users { get; set; }

		public DbSet<RefreshToken> RefreshTokens { get; set; }

		public PantrifyDbContext(DbContextOptions<PantrifyDbContext> options) : base(options) { }
	}
}