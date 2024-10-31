using Pantrify.API.Models;

namespace Pantrify.API.Repositories
{
	public interface IUserRepository
	{
		Task<User?> GetById(int id);

		Task<User?> Create(User user);

		Task<User?> AuthenticateUser(string email, string password);
	}
}