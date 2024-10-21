using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pantrify.API.Model
{
	[Table(nameof(Recipe))]
	public class Recipe
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; } = null!;

		[Required]
		public int Duration { get; set; }

		[Required]
		public int Difficulty { get; set; }

		[Required]
		public int NumServings { get; set; }

		// Collection navigation containing dependents
		public List<RecipeIngredient> Ingredients { get; set; } = null!;

		// Collection navigation containing dependents
		public List<RecipeInstruction> Instructions { get; set; } = null!;

		// Collection navigation containing dependents
		public RecipeImage Image { get; set; } = null!;


	}
}