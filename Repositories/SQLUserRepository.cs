using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Model;

namespace Pantrify.API.Repositories
{
	public class SQLUserRepository : IUserRepository
	{
		private readonly AuthDbcontext authDbcontext;

		public SQLUserRepository(
			AuthDbcontext authDbcontext // Inject AuthDbContext
		)
		{
			this.authDbcontext = authDbcontext;
		}

		public async Task<User?> Create(User user)
		{
			User? foundUser = await this.authDbcontext.Users
				.Where(u => u.Email.ToLower() == user.Email.ToLower())
				.FirstOrDefaultAsync();

			// A user with same Email is found
			if (foundUser != null)
			{
				return null;
			}

			// Add user
			await this.authDbcontext.Users.AddAsync(user);

			// Persist changes
			await this.authDbcontext.SaveChangesAsync();

			return user;
		}

		public async Task<User?> AuthenticateUser(User user)
		{
			User? foundUser = await this.authDbcontext.Users
				.Where(u => u.Email.ToLower() == user.Email.ToLower())
				.FirstOrDefaultAsync();

			// Check for existence
			if (foundUser == null)
			{
				return null;
			}

			// Check if password matches
			if (string.Equals(foundUser.PasswordHash, user.PasswordHash))
			{
				return user;
			}

			return null;
		}
	}
}