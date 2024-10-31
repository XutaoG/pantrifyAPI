namespace Pantrify.API.Dto.Auth
{
	public class JwtResponse
	{
		public string Token { get; set; } = null!;

		public string RefreshToken { get; set; } = null!;

		public string TokenType { get; set; } = "Bearer";

		public DateTime TokenExpiryTime { get; set; }

		public DateTime RefreshTokenExpiryTime { get; set; }
	}
}