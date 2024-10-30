using Pantrify.API.Model;

namespace Pantrify.API.Repositories
{
	public interface IIngredientRepository
	{
		Task<List<Ingredient>> GetAll();

		Task<Ingredient?> GetbyId(int id);

		Task<Ingredient> Create(Ingredient ingredient);

		Task<Ingredient?> UpdateById(int id, Ingredient ingredient);

		Task<Ingredient?> DeleteById(int id);
	}
}