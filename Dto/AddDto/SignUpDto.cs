using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Add.Dto
{
	public class SignUpDto
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