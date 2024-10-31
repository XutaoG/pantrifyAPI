using Microsoft.AspNetCore.Identity;
using Pantrify.API.Model;

namespace Pantrify.API.Services
{
	public class PasswordHashService
	{
		private readonly PasswordHasher<User> passwordHasher;
		private readonly User user;

		public PasswordHashService()
		{
			this.passwordHasher = new PasswordHasher<User>();
			this.user = new User();
		}

		public string HashPassword(string password)
		{
			return passwordHasher.HashPassword(user, password);
		}

		public bool VerifyPassword(string hashedPassword, string password)
		{
			PasswordVerificationResult res = passwordHasher.VerifyHashedPassword(user, hashedPassword, password);

			return res == PasswordVerificationResult.Success;
		}
	}
}