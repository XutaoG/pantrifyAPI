using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Dtos
{
	public class AddIngredientRequest
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