using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Add.Dto
{
	public class UpdateIngredientDto
	{
		[Required]
		public string Name { get; set; } = null!;

		[Required]
		public string IngredientType { get; set; } = null!;

		[Required]
		public bool IsAvailable { get; set; }

		[Required]
		public bool IsInCart { get; set; }

		public DateTime? DateExpired { get; set; }
	}
}