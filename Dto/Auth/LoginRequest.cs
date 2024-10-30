using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Dto.Auth
{
	public class LoginRequest
	{
		[Required]
		public string Email { get; set; } = null!;

		[Required]
		public string Password { get; set; } = null!;
	}
}