using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Model;
using Pantrify.API.Utils;

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

		public async Task<User?> AuthenticateUser(string email, string password)
		{
			User? foundUser = await this.authDbcontext.Users
				.Where(u => u.Email.ToLower() == email.ToLower())
				.FirstOrDefaultAsync();

			// Check for existence
			if (foundUser == null)
			{
				return null;
			}

			// Check if password matches
			if (PasswordHasherService.VerifyPassword(foundUser.PasswordHash, password))
			{
				return foundUser;
			}

			return null;
		}
	}
}