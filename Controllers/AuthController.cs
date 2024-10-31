using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Pantrify.API.Dtos;
using Pantrify.API.Models;
using Pantrify.API.Repositories;
using Pantrify.API.Services;

namespace Pantrify.API.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IUserRepository userRepository;
		private readonly ITokenRepository tokenRepository;
		private readonly IConfiguration configuration;
		private readonly PasswordHashService passwordHasherService;
		private readonly JwtService jwtService;

		public AuthController(
			IUserRepository userRepository,
			ITokenRepository tokenRepository,
			IConfiguration configuration,
			PasswordHashService passwordHasherService,
			JwtService jwtService
		)
		{
			this.userRepository = userRepository;
			this.tokenRepository = tokenRepository;
			this.configuration = configuration;
			this.passwordHasherService = passwordHasherService;
			this.jwtService = jwtService;
		}

		[Route("sign-up")]
		[HttpPost]
		public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest)
		{
			if (!ModelState.IsValid)
			{
				// 400
				return BadRequest(ModelState);
			}

			User? user = new User()
			{
				Email = signUpRequest.Email,
				FirstName = signUpRequest.FirstName,
				LastName = signUpRequest.LastName,
				PasswordHash = this.passwordHasherService.HashPassword(signUpRequest.Password)
			};

			user = await this.userRepository.Create(user);

			if (user == null)
			{
				ModelState.AddModelError("Email", "Email already used");

				// 400
				return BadRequest(ModelState);
			}

			// 200
			return Ok();
		}

		[Route("login")]
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
		{
			if (!ModelState.IsValid)
			{
				// 400
				return BadRequest(ModelState);
			}

			User? user = await this.userRepository.AuthenticateUser(loginRequest.Email, loginRequest.Password);

			if (user == null)
			{
				// 401
				return Unauthorized();
			}

			// Generate JWT and refresh token
			Jwt jwt = this.jwtService.GenerateJwt(user);
			RefreshToken refreshToken = await this.jwtService.GenerateRefreshTokenAsync(user);

			JwtResponse response = new JwtResponse()
			{
				Token = jwt.Token,
				RefreshToken = refreshToken.Token,
				TokenExpiryTime = jwt.ExpiryTime,
				RefreshTokenExpiryTime = refreshToken.ExpiryTime
			};

			// 200
			return Ok(response);
		}

		[Route("refresh")]
		[HttpPost]
		public async Task<IActionResult> Refresh([FromBody] TokenRequest tokenRequest)
		{
			// Get claims principal
			ClaimsPrincipal principal = this.jwtService.GetPrincipalFromExpiredToken(tokenRequest.Token);

			if (principal == null)
			{
				ModelState.AddModelError("Token", "Invalid JWT");
				return BadRequest(ModelState);
			}

			// Get refresh token
			RefreshToken? refreshToken = await this.tokenRepository.GetByToken(tokenRequest.RefreshToken);

			if (refreshToken == null || refreshToken.ExpiryTime < DateTime.UtcNow)
			{
				ModelState.AddModelError("Refresh token", "Invalid refresh token");
				return Unauthorized(ModelState);
			}

			// Get user ID claim
			Claim? userIdClaim = principal.FindFirst("userId");

			if (userIdClaim == null)
			{
				ModelState.AddModelError("Token", "Invalid JWT");
				return Unauthorized(ModelState);
			}


			// Check user ID and refresh token user ID
			if (int.TryParse(userIdClaim.Value, out int claimUserId))
			{
				if (claimUserId == refreshToken.UserId)
				{
					User? user = await this.userRepository.GetById(claimUserId);
					if (user == null)
					{
						// 404
						ModelState.AddModelError("User", "Account not found");
						return NotFound(ModelState);
					}

					// Generate new JWT
					Jwt newJwt = this.jwtService.GenerateJwt(user);
					// Generate new refresh token
					RefreshToken newRefreshToken = await this.jwtService.GenerateRefreshTokenAsync(user);

					// Delete old refresh token
					await this.tokenRepository.DeleteByToken(refreshToken.Token);

					JwtResponse response = new JwtResponse()
					{
						Token = newJwt.Token,
						RefreshToken = newRefreshToken.Token,
						TokenExpiryTime = newJwt.ExpiryTime,
						RefreshTokenExpiryTime = newRefreshToken.ExpiryTime
					};

					// 200
					return Ok(response);
				}
			}

			ModelState.AddModelError("Token", "Invalid JWT");
			return Unauthorized(ModelState);
		}
	}
}