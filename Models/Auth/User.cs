using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Email { get; set; } = null!;

		[Required]
		public string FirstName { get; set; } = null!;

		[Required]
		public string LastName { get; set; } = null!;

		[Required]
		public string PasswordHash { get; set; } = null!;

		[Required]
		public bool EmailConfirmed { get; set; } = false;
	}
}