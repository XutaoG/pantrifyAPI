using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Dtos
{
	public class TokenRequest
	{
		[Required]
		public string Token { get; set; } = null!;

		[Required]
		public string RefreshToken { get; set; } = null!;
	}
}