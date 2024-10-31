using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Dto.Auth
{
	public class TokenRequest
	{
		[Required]
		public string Token { get; set; } = null!;

		[Required]
		public string RefreshToken { get; set; } = null!;
	}
}