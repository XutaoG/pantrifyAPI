namespace Pantrify.API.Dto
{
	public class IngredientResponse
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public string IngredientType { get; set; } = null!;

		public bool IsAvailable { get; set; }

		public bool IsInCart { get; set; }

		public int UserId { get; set; }

		public DateTime DateAdded { get; set; }

		public DateTime? DateExpired { get; set; }
	}
}