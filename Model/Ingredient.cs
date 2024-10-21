using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pantrify.API.Model
{
	[Table(nameof(Ingredient))]
	public class Ingredient : IngredientBase
	{
		[Required]
		public bool IsAvailable { get; set; }

		[Required]
		public bool IsInCart { get; set; }

		[Required]
		public DateTime dateAdded { get; set; }

		[Required]
		public DateTime dateExpired { get; set; }
	}
}