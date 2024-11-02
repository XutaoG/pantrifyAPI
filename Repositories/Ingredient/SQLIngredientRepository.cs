using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Models;

namespace Pantrify.API.Repositories
{
	public class SQLIngredientRepository : IIngredientRepository
	{
		private readonly PantrifyDbContext pantrifyDbContext;

		public SQLIngredientRepository(
			PantrifyDbContext pantrifyDbContext // Inject PantrifyDbContext
			)
		{
			this.pantrifyDbContext = pantrifyDbContext;
		}

		public async Task<List<Ingredient>> GetByUser(
			int userId,
			string? name,
			string? ingredientType,
			bool? isAvailable,
			bool? isInCart,
			string? sortBy,
			bool? isAscending,
			int? pageNumber,
			int? pageSize
			)
		{
			IQueryable<Ingredient> ingredients = this.pantrifyDbContext.Ingredients.Where(
				ing => ing.UserId == userId &&
				ing.Name.ToLower().Contains(name == null ? "" : name.ToLower())
				).AsQueryable();

			// Filter ingredient type
			if (!string.IsNullOrWhiteSpace(ingredientType))
			{
				ingredients = ingredients.Where(ing => ing.IngredientType == ingredientType);
			}

			// Filter by inventory availability
			if (isAvailable != null)
			{
				ingredients = ingredients.Where(ing => ing.IsAvailable == isAvailable);
			}

			// Filter by cart availability
			if (isInCart != null)
			{
				ingredients = ingredients.Where(ing => ing.IsInCart == isInCart);
			}

			if (sortBy != null)
			{
				// Sort by name
				if (sortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
				{
					ingredients = isAscending ?? true ?
						ingredients.OrderBy(ing => ing.Name) :
						ingredients.OrderByDescending(ing => ing.Name);
				}
				// Sort by date added
				else if (sortBy.Equals("dateAdded", StringComparison.OrdinalIgnoreCase))
				{
					ingredients = isAscending ?? true ?
						ingredients.OrderBy(ing => ing.DateAdded) :
						ingredients.OrderByDescending(ing => ing.DateAdded);
				}
				// Sort by date expired
				else if (sortBy.Equals("dateExpired", StringComparison.OrdinalIgnoreCase))
				{
					ingredients = isAscending ?? true ?
						ingredients.OrderBy(ing => ing.DateExpired) :
						ingredients.OrderByDescending(ing => ing.DateExpired);
				}
			}

			// Pagination
			int skipResult = ((pageNumber ?? 1) - 1) * (pageSize ?? 12);
			ingredients = ingredients.Skip(skipResult).Take(pageSize ?? 12);

			return await ingredients.ToListAsync();
		}

		public async Task<Ingredient?> GetById(int id)
		{
			return await this.pantrifyDbContext.Ingredients
				.Where(ing => ing.Id == id)
				.FirstOrDefaultAsync();
		}

		public async Task<Ingredient> Create(Ingredient ingredient)
		{
			// Add ingredient
			await this.pantrifyDbContext.Ingredients.AddAsync(ingredient);

			// Persist changes
			await this.pantrifyDbContext.SaveChangesAsync();

			return ingredient;
		}

		public async Task<Ingredient?> UpdateById(int id, Ingredient ingredient)
		{
			// Check if ID exists
			Ingredient? foundIngredient = await GetById(id);

			if (foundIngredient == null)
			{
				return null;
			}

			// Update ingredient
			foundIngredient.Name = ingredient.Name;
			foundIngredient.IngredientType = ingredient.IngredientType;
			foundIngredient.IsAvailable = ingredient.IsAvailable;
			foundIngredient.IsInCart = ingredient.IsInCart;
			foundIngredient.DateAdded = ingredient.DateAdded;
			foundIngredient.DateExpired = ingredient.DateExpired;

			// Persist changes
			await this.pantrifyDbContext.SaveChangesAsync();

			return foundIngredient;
		}

		public async Task<Ingredient?> DeleteById(int id)
		{
			// Check if ID exists
			Ingredient? foundIngredient = await GetById(id);

			if (foundIngredient == null)
			{
				return null;
			}

			// Delete ingredient
			this.pantrifyDbContext.Ingredients.Remove(foundIngredient);

			// Persist changes
			await this.pantrifyDbContext.SaveChangesAsync();

			return foundIngredient;
		}

		public async Task<Ingredient?> GetByName(string name)
		{
			return await this.pantrifyDbContext.Ingredients
				.Where(ing => ing.Name.ToLower() == name.ToLower())
				.FirstOrDefaultAsync();
		}
	}
}