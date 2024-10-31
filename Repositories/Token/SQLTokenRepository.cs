using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Models;

namespace Pantrify.API.Repositories
{
	public class SQLTokenRepository : ITokenRepository
	{
		private readonly PantrifyDbContext dbContext;

		public SQLTokenRepository(PantrifyDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<RefreshToken> Create(RefreshToken refreshToken)
		{
			// Add token
			await this.dbContext.RefreshTokens.AddAsync(refreshToken);

			// Persist changes
			await this.dbContext.SaveChangesAsync();

			return refreshToken;
		}


		public async Task<RefreshToken?> GetByToken(string token)
		{
			return await this.dbContext.RefreshTokens
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
			this.dbContext.RefreshTokens.Remove(foundRefreshToken);

			// Persist changes
			await this.dbContext.SaveChangesAsync();

			return foundRefreshToken;
		}
	}
}