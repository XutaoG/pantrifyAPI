using Microsoft.AspNetCore.Identity;
using Pantrify.API.Model;

namespace Pantrify.API.Utils
{
	public class PasswordHasherService
	{
		private readonly static PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
		private readonly static User user = new User();

		public static string HashPassword(string password)
		{
			return passwordHasher.HashPassword(user, password);
		}

		public static bool verifyPassword(string hashedPassword, string password)
		{
			PasswordVerificationResult res = passwordHasher.VerifyHashedPassword(user, hashedPassword, password);

			return res == PasswordVerificationResult.Success;
		}

	}
}