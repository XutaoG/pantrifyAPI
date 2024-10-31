using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Models;
using Pantrify.API.Services;

namespace Pantrify.API.Repositories
{
	public class SQLUserRepository : IUserRepository
	{
		private readonly PantrifyDbContext dbContext;
		private readonly PasswordHashService passwordHashService;

		public SQLUserRepository(
			PantrifyDbContext dbContext,
			PasswordHashService passwordHashService
		)
		{
			this.dbContext = dbContext;
			this.passwordHashService = passwordHashService;
		}

		public async Task<User?> Create(User user)
		{
			User? foundUser = await this.dbContext.Users
				.Where(u => u.Email.ToLower() == user.Email.ToLower())
				.FirstOrDefaultAsync();

			// A user with same Email is found
			if (foundUser != null)
			{
				return null;
			}

			// Add user
			await this.dbContext.Users.AddAsync(user);

			// Persist changes
			await this.dbContext.SaveChangesAsync();

			return user;
		}

		public async Task<User?> AuthenticateUser(string email, string password)
		{
			User? foundUser = await this.dbContext.Users
				.Where(u => u.Email.ToLower() == email.ToLower())
				.FirstOrDefaultAsync();

			// Check for existence
			if (foundUser == null)
			{
				return null;
			}

			// Check if password matches
			if (this.passwordHashService.VerifyPassword(foundUser.PasswordHash, password))
			{
				return foundUser;
			}

			return null;
		}

		public async Task<User?> GetById(int id)
		{
			return await this.dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
		}
	}
}