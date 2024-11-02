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

		public async Task<List<Recipe>> GetByUser(
			int userId,
			string? name,
			int? difficulty,
			int? minDuration,
			int? maxDuration,
			string? sortBy,
			bool? isAscending
			)
		{
			IQueryable<Recipe> recipes = this.dbContext.Recipes.Where(
				recipe => recipe.UserId == userId &&
				recipe.Name.ToLower().Contains(name == null ? "" : name.ToLower()))
				.Include("Instructions")
				.Include("Ingredients")
				.AsQueryable();

			// Filter difficulty
			if (difficulty != null)
			{
				recipes = recipes.Where(recipe => recipe.Difficulty == difficulty);
			}

			// Filter by duration
			if (minDuration != null)
			{
				recipes = recipes.Where(recipes => recipes.Duration >= minDuration);
			}

			if (maxDuration != null)
			{
				recipes = recipes.Where(recipe => recipe.Duration <= maxDuration);
			}

			if (sortBy != null)
			{
				// Sort by name
				if (sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
				{
					recipes = isAscending ?? true ?
						recipes.OrderBy(recipe => recipe.Name) :
						recipes.OrderByDescending(recipe => recipe.Name);
				}
				// Sort by difficulty
				if (sortBy.Equals("difficulty", StringComparison.OrdinalIgnoreCase))
				{
					recipes = isAscending ?? true ?
						recipes.OrderBy(recipe => recipe.Difficulty) :
						recipes.OrderByDescending(recipe => recipe.Difficulty);
				}
				// Sort by duration
				if (sortBy.Equals("duration", StringComparison.OrdinalIgnoreCase))
				{
					recipes = isAscending ?? true ?
						recipes.OrderBy(recipe => recipe.Duration) :
						recipes.OrderByDescending(recipe => recipe.Duration);
				}
			}

			return await recipes.ToListAsync();
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
			foundRecipe.DateModified = DateTime.UtcNow;

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