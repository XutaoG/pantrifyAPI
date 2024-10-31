using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Model;

namespace Pantrify.API.Repositories
{
	public class SQLTokenRepository : ITokenRepository
	{
		private readonly AuthDbcontext authDbcontext;

		public SQLTokenRepository(AuthDbcontext authDbcontext)
		{
			this.authDbcontext = authDbcontext;
		}

		public async Task<RefreshToken> Create(RefreshToken refreshToken)
		{
			// Add token
			await this.authDbcontext.RefreshTokens.AddAsync(refreshToken);

			// Persist changes
			await this.authDbcontext.SaveChangesAsync();

			return refreshToken;
		}


		public async Task<RefreshToken?> GetByToken(string token)
		{
			return await this.authDbcontext.RefreshTokens
				.Where(t => t.Token == token)
				.FirstOrDefaultAsync();
		}

		public async Task<RefreshToken?> DeleteByToken(string token)
		{
			// Check if ID exists
			RefreshToken? foundRefreshToken = await GetByToken(token);

			if (foundRefreshToken == null)
			{
				return null;
			}

			// Delete ingredient
			this.authDbcontext.RefreshTokens.Remove(foundRefreshToken);

			// Persist changes
			await this.authDbcontext.SaveChangesAsync();

			return foundRefreshToken;
		}
	}
}