namespace Pantrify.API.Dtos
{
	public class IngredientAvailabilityResponse
	{
		public string Name { get; set; } = null!;

		public bool Availability { get; set; }
	}
}