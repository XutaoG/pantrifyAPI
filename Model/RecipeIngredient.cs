using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pantrify.API.Models
{
	[Table(nameof(RecipeIngredient))]
	public class RecipeIngredient : IngredientBase
	{
		public int? Quantity { get; set; }

		public string? QuantityUnit { get; set; } = null!;

		[ForeignKey(nameof(Recipe))] // Specifies property as foreign key
		public int RecipeId { get; set; }

		// Reference navigation to principal
		[JsonIgnore] // Prevent property from being serialized
		public Recipe Recipe { get; set; } = null!;
	}
}