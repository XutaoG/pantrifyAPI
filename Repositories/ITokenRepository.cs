using Pantrify.API.Models;

namespace Pantrify.API.Repositories
{
	public interface ITokenRepository
	{
		Task<RefreshToken> Create(RefreshToken refreshToken);

		Task<RefreshToken?> GetByToken(string token);

		Task<RefreshToken?> DeleteByToken(string token);
	}
}