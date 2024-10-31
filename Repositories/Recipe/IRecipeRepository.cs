using Pantrify.API.Models;

namespace Pantrify.API.Repositories
{
	public interface IRecipeRepository
	{
		Task<List<Recipe>> GetByUser(int userId);

		Task<Recipe?> GetById(int id);

		Task<Recipe> Create(Recipe recipe);

		Task<Recipe?> UpdateById(int id, Recipe recipe);

		Task<Recipe?> DeleteById(int id);
	}
}