using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pantrify.API.Models;
using Pantrify.API.Repositories;

namespace Pantrify.API.Services
{
	public class JwtService
	{
		private readonly IConfiguration configuration;
		private readonly ITokenRepository tokenRepository;

		public JwtService(
			IConfiguration configuration,
			ITokenRepository tokenRepository)
		{
			this.configuration = configuration;
			this.tokenRepository = tokenRepository;
		}

		public Jwt GenerateJwt(User user)
		{
			byte[] secretKey = Encoding.ASCII.GetBytes(this.configuration.GetValue<string>("secret_key") ?? "");
			DateTime expiryTime = DateTime.UtcNow.AddMinutes(30);

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

			SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity([
					new Claim(ClaimTypes.Email, user.Email),
					new Claim("userId", user.Id.ToString())
				]),
				NotBefore = DateTime.UtcNow,
				Expires = expiryTime,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature),
			};

			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

			Jwt jwt = new Jwt()
			{
				Token = tokenHandler.WriteToken(token),
				ExpiryTime = expiryTime
			};

			return jwt;
		}

		public async Task<RefreshToken> GenerateRefreshTokenAsync(User user)
		{
			RefreshToken refreshToken = new RefreshToken()
			{
				Token = Guid.NewGuid().ToString(),
				UserId = user.Id,
				CreateTime = DateTime.UtcNow,
				ExpiryTime = DateTime.UtcNow.AddDays(7)
			};

			// Add refresh token to DB
			refreshToken = await this.tokenRepository.Create(refreshToken);

			return refreshToken;
		}

		public ClaimsPrincipal GetPrincipalFromExpiredToken(string jwt)
		{
			byte[] secretKey = Encoding.ASCII.GetBytes(this.configuration.GetValue<string>("secret_key") ?? "");

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

			// Create token validation parameter
			TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(secretKey),
				ValidateLifetime = false, // Don't validate lifetime because token is assumed to be expired
				ValidateAudience = false,
				ValidateIssuer = false,
			};

			ClaimsPrincipal principal = tokenHandler.ValidateToken(jwt, tokenValidationParameters, out SecurityToken securityToken);

			return principal;
		}

		public int? GetUserIdFromClaims(List<Claim> claims)
		{
			Claim? userIdClaim = claims.FirstOrDefault(c => c.Type == "userId");

			if (userIdClaim == null)
			{
				return null;
			}

			if (int.TryParse(userIdClaim.Value, out int claimUserId))
			{
				return claimUserId;
			}

			return null;
		}
	}
}