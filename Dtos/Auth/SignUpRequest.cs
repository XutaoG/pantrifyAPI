using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Dtos
{
	public class SignUpRequest
	{
		[Required]
		public string Email { get; set; } = null!;

		[Required]
		public string FirstName { get; set; } = null!;

		[Required]
		public string LastName { get; set; } = null!;

		[Required]
		public string Password { get; set; } = null!;
	}
}