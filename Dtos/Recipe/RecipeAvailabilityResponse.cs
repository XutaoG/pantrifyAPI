namespace Pantrify.API.Dtos
{
	public class RecipeAvailbilityResponse
	{
		public List<IngredientAvailabilityResponse> IngredientAvailabilities { get; set; } = null!;
	}

	public class IngredientAvailabilityResponse
	{
		public string Name { get; set; } = null!;

		public bool Availability { get; set; }
	}
}