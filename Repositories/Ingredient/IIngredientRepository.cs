using Pantrify.API.Models;

namespace Pantrify.API.Repositories
{
	public interface IIngredientRepository
	{
		Task<List<Ingredient>> GetByUser(
			int userId,
			string? name,
			string? ingredientType,
			bool? isAvailable,
			bool? isInCart,
			string? sortBy,
			bool? isAscending
			);

		Task<Ingredient?> GetById(int id);

		Task<Ingredient> Create(Ingredient ingredient);

		Task<Ingredient?> UpdateById(int id, Ingredient ingredient);

		Task<Ingredient?> DeleteById(int id);

		Task<Ingredient?> GetByName(string name);
	}
}