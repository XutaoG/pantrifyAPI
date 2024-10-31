using System.ComponentModel.DataAnnotations;

namespace Pantrify.API.Models
{
	public class RefreshToken
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Token { get; set; } = null!;

		[Required]
		public int UserId { get; set; }

		[Required]
		public DateTime CreateTime { get; set; }

		[Required]
		public DateTime ExpiryTime { get; set; }
	}
}