using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Dtos
{
	public class AddRecipeRequest

	{
		[Required]
		public string Name { get; set; } = null!;

		[Required]
		public int Duration { get; set; }

		[Required]
		public int Difficulty { get; set; }

		[Required]
		public int NumServings { get; set; }

		[Required]
		public List<AddRecipeIngredientRequest> Ingredients { get; set; } = null!;

		[Required]
		public List<string> Instructions { get; set; } = null!;

		[Required]
		public List<IFormFile> Images { get; set; } = null!;
	}

	public class AddRecipeIngredientRequest
	{
		[Required]
		public string Name { get; set; } = null!;

		[Required]
		public string IngredientType { get; set; } = null!;

		public int? Quantity { get; set; }

		public string? QuantityUnit { get; set; } = null!;
	}
}