using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pantrify.API.Model
{
	[Table(nameof(RecipeIngredient))]
	public class RecipeIngredient : IngredientBase
	{
		[Key]
		public int Id { get; set; }

		public int? Quantity { get; set; }

		public string? QuantityUnit { get; set; } = null!;

		[Required]
		public string ingredientType { get; set; } = null!;

		[ForeignKey(nameof(Recipe))] // Specifies property as foreign key
		public int RecipeId { get; set; }

		// Reference navigation to principal
		[JsonIgnore] // Prevent property from being serialized
		public Recipe recipe { get; set; } = null!;
	}
}