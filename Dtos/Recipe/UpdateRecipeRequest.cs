using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Dtos
{
	public class UpdateRecipeRequest
	{
		[Required]
		public string Name { get; set; } = null!;

		public string? Description { get; set; } = null!;

		[Required]
		public int Duration { get; set; }

		[Required]
		public int Difficulty { get; set; }

		[Required]
		public int NumServings { get; set; }

		[Required]
		public List<UpdateRecipeIngredientRequest> Ingredients { get; set; } = null!;

		[Required]
		public List<string> Instructions { get; set; } = null!;

		public List<IFormFile>? Images { get; set; }
	}

	public class UpdateRecipeIngredientRequest
	{
		[Required]
		public string Name { get; set; } = null!;

		[Required]
		public string IngredientType { get; set; } = null!;

		public int? QuantityWhole { get; set; }

		public string? QuantityFraction { get; set; }

		public string? QuantityUnit { get; set; } = null!;
	}
}