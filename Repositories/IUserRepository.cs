using Pantrify.API.Model;

namespace Pantrify.API.Repositories
{
	public interface IUserRepository
	{
		Task<User?> Create(User user);

		Task<User?> AuthenticateUser(User user);
	}
}