namespace Pantrify.API.Model
{
	public class Jwt
	{
		public string Token { get; set; } = null!;

		public DateTime ExpiryTime { get; set; }
	}
}