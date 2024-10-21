using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Model;

namespace Pantrify.API.Repository
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

		public async Task<List<Ingredient>> GetAll()
		{
			return await this.pantrifyDbContext.Ingredients.ToListAsync();
		}

		public async Task<Ingredient?> GetbyId(int id)
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
			Ingredient? foundIngredient = await GetbyId(id);

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
			Ingredient? foundIngredient = await GetbyId(id);

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
	}
}