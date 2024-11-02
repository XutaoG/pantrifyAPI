
using Pantrify.API.Models;

namespace Pantrify.API.Dtos
{
	public class IngredientListResponse
	{
		public List<IngredientResponse> Ingredients { get; set; } = null!;

		public int TotalCount { get; set; }

		public int PageNumber { get; set; }

		public int PageSize { get; set; }
	}
}