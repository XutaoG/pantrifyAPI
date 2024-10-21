using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pantrify.API.Model
{
	[Table(nameof(RecipeInstruction))]
	public class RecipeInstruction
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Instruction { get; set; } = null!;

		[Required]
		public int Step { get; set; }

		[ForeignKey(nameof(Recipe))] // Specifies property as foreign key
		public int RecipeId { get; set; }

		// Reference navigation to principal
		[JsonIgnore] // Prevent property from being serialized
		public Recipe Recipe { get; set; } = null!;
	}
}