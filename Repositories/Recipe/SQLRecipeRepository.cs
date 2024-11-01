using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Models;

namespace Pantrify.API.Repositories
{
	public class SQLRecipeRepository : IRecipeRepository
	{
		private readonly PantrifyDbContext dbContext;

		public SQLRecipeRepository(PantrifyDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<List<Recipe>> GetByUser(int userId)
		{
			return await this.dbContext.Recipes.Where(recipe => recipe.UserId == userId)
				.Include("Instructions")
				.Include("Ingredients")
				.ToListAsync();
		}

		public async Task<Recipe?> GetById(int id)
		{
			return await this.dbContext.Recipes.Where(recipe => recipe.Id == id)
				.Include("Instructions")
				.Include("Ingredients")
				.FirstOrDefaultAsync();
		}

		public async Task<Recipe> Create(Recipe recipe)
		{
			// Add recipe
			await this.dbContext.Recipes.AddAsync(recipe);

			// Persist changes
			await this.dbContext.SaveChangesAsync();

			return recipe;
		}

		public async Task<Recipe?> UpdateById(int id, Recipe recipe)
		{
			// Check if ID exists
			Recipe? foundRecipe = await GetById(id);

			if (foundRecipe == null)
			{
				return null;
			}

			// Update recipe
			foundRecipe.Name = recipe.Name;
			foundRecipe.Duration = recipe.Duration;
			foundRecipe.Difficulty = recipe.Difficulty;
			foundRecipe.NumServings = recipe.NumServings;
			foundRecipe.Ingredients = recipe.Ingredients;
			foundRecipe.Instructions = recipe.Instructions;

			// Persist changes
			await this.dbContext.SaveChangesAsync();

			return foundRecipe;
		}

		public async Task<Recipe?> DeleteById(int id)
		{
			// Check if ID exists
			Recipe? foundRecipe = await GetById(id);

			if (foundRecipe == null)
			{
				return null;
			}

			// Delete recipe
			this.dbContext.Recipes.Remove(foundRecipe);

			// Persist changes
			await this.dbContext.SaveChangesAsync();

			return foundRecipe;
		}
	}
}