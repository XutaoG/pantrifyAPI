using Microsoft.EntityFrameworkCore;
using Pantrify.API.Model;

namespace Pantrify.API.Data
{
	public class AuthDbcontext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public DbSet<RefreshToken> RefreshTokens { get; set; }

		public AuthDbcontext(DbContextOptions<AuthDbcontext> options) : base(options) { }


	}
}