using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pantrify.API.Models
{
	[Table(nameof(RecipeImage))]
	public class RecipeImage
	{
		[Key]
		public int Id { get; set; }

		[NotMapped]
		public IFormFile File { get; set; } = null!;

		[Required]
		public string Path { get; set; } = null!;

		[Required]
		public int Order { get; set; }

		[ForeignKey(nameof(Recipe))] // Specifies property as foreign key
		public int RecipeId { get; set; }

		// Reference navigation to principal
		[JsonIgnore] // Prevent property from being serialized
		public Recipe Recipe { get; set; } = null!;
	}
}