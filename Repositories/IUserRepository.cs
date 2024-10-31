using Pantrify.API.Model;

namespace Pantrify.API.Repositories
{
	public interface IUserRepository
	{
		Task<User?> GetById(int id);

		Task<User?> Create(User user);

		Task<User?> AuthenticateUser(string email, string password);
	}
}