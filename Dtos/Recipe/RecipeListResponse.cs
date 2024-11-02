
namespace Pantrify.API.Dtos
{
	public class RecipeListResponse
	{
		public List<RecipeResponse> Recipes { get; set; } = null!;

		public int TotalCount { get; set; }

		public int PageNumber { get; set; }

		public int PageSize { get; set; }
	}
}