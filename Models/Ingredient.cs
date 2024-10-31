using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pantrify.API.Models
{
	[Table(nameof(Ingredient))]
	public class Ingredient : IngredientBase
	{
		[Required]
		public bool IsAvailable { get; set; }

		[Required]
		public bool IsInCart { get; set; }

		[Required]
		public DateTime DateAdded { get; set; }

		public DateTime? DateExpired { get; set; }

		[ForeignKey(nameof(User))] // Specifies property as foreign key
		public int UserId { get; set; }

		public Ingredient()
		{
			this.DateAdded = DateTime.UtcNow;
		}
	}
}