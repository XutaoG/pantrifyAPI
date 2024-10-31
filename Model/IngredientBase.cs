using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Models
{
	public abstract class IngredientBase
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; } = null!;

		[Required]
		public string IngredientType { get; set; } = null!;
	}
}