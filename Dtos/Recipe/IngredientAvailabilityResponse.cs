namespace Pantrify.API.Dtos
{
	public class IngredientAvailabilityResponse : RecipeIngredientResponse
	{
		public bool IsAvailable { get; set; }

		public int? IngredientId { get; set; }
	}
}