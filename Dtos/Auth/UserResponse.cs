
namespace Pantrify.API.Dtos
{
	public class UserResponse
	{
		public int Id { get; set; }

		public string Email { get; set; } = null!;

		public string FirstName { get; set; } = null!;

		public string LastName { get; set; } = null!;
	}
}