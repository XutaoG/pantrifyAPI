using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
		private readonly PasswordHashService passwordHasherService;
		private readonly JwtService jwtService;
		private readonly IMapper mapper;

		public AuthController(
			IUserRepository userRepository,
			ITokenRepository tokenRepository,
			PasswordHashService passwordHasherService,
			JwtService jwtService,
			IMapper mapper
		)
		{
			this.userRepository = userRepository;
			this.tokenRepository = tokenRepository;
			this.passwordHasherService = passwordHasherService;
			this.jwtService = jwtService;
			this.mapper = mapper;
		}

		[Route("sign-up")]
		[HttpPost]
		public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpRequest)
		{
			if (!ModelState.IsValid)
			{
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

				// return BadRequest(ModelState);

				return Conflict(ModelState);
			}

			return Ok();
		}

		[Route("login")]
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			User? user = await this.userRepository.AuthenticateUser(loginRequest.Email, loginRequest.Password);

			if (user == null)
			{
				return Unauthorized();
			}

			// Generate JWT and refresh token
			Jwt jwt = this.jwtService.GenerateJwt(user);
			RefreshToken refreshToken = await this.jwtService.GenerateRefreshTokenAsync(user);

			JwtResponse response = new JwtResponse()
			{
				// Token = jwt.Token,
				// RefreshToken = refreshToken.Token,
				TokenExpiryTime = jwt.ExpiryTime,
				RefreshTokenExpiryTime = refreshToken.ExpiryTime
			};

			// Configure cookie options
			CookieOptions cookieOptions = new CookieOptions()
			{
				HttpOnly = true,
				SameSite = SameSiteMode.Strict,
			};

			// Set persistent or non persistent cookie
			if (loginRequest.RememberMe)
			{
				cookieOptions.Expires = DateTime.UtcNow.AddDays(7);
			}

			// Add JWT and refresh token in response cookie
			HttpContext.Response.Cookies.Append("X-Refresh-Token", refreshToken.Token, cookieOptions);
			HttpContext.Response.Cookies.Append("X-Access-Token", jwt.Token, cookieOptions);

			return Ok(response);
		}

		[Route("refresh")]
		[HttpPost]
		public async Task<IActionResult> Refresh()
		{
			if (!Request.Cookies.TryGetValue("X-Access-Token", out string? jwt) || !Request.Cookies.TryGetValue("X-Refresh-Token", out string? refreshToken))
			{
				ModelState.AddModelError("Token", "Missing JWT or refresh token");
				return BadRequest();
			}

			// Get claims principal
			ClaimsPrincipal principal = this.jwtService.GetPrincipalFromExpiredToken(jwt);

			if (principal == null)
			{
				ModelState.AddModelError("Token", "Invalid JWT");
				return BadRequest(ModelState);
			}

			// Get refresh token
			RefreshToken? foundRefreshToken = await this.tokenRepository.GetByToken(refreshToken);

			if (foundRefreshToken == null || foundRefreshToken.ExpiryTime < DateTime.UtcNow)
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
				if (claimUserId == foundRefreshToken.UserId)
				{
					User? user = await this.userRepository.GetById(claimUserId);
					if (user == null)
					{
						// 404
						ModelState.AddModelError("User", "Account not found");
						return NotFound(ModelState);
					}


					Jwt newJwt = this.jwtService.GenerateJwt(user);
					RefreshToken newRefreshToken = await this.jwtService.GenerateRefreshTokenAsync(user);

					JwtResponse response = new JwtResponse()
					{
						// Token = jwt.Token,
						// RefreshToken = refreshToken.Token,
						TokenExpiryTime = newJwt.ExpiryTime,
						RefreshTokenExpiryTime = newRefreshToken.ExpiryTime
					};

					// Configure cookie options
					CookieOptions cookieOptions = new CookieOptions()
					{
						HttpOnly = true,
						SameSite = SameSiteMode.Strict,
					};

					// Add JWT and refresh token in response cookie
					HttpContext.Response.Cookies.Append("X-Refresh-Token", newRefreshToken.Token, cookieOptions);
					HttpContext.Response.Cookies.Append("X-Access-Token", newJwt.Token, cookieOptions);

					return Ok(response);
				}
			}

			ModelState.AddModelError("Token", "Invalid JWT");
			return Unauthorized(ModelState);
		}

		[Route("user")]
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetUser()
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				return Unauthorized();
			}

			User? foundUser = await this.userRepository.GetById((int)userId);

			if (foundUser == null)
			{
				return NotFound();
			}

			// Map model to Dto
			UserResponse response = this.mapper.Map<UserResponse>(foundUser);

			return Ok(response);
		}

		[Route("logout")]
		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			if (Request.Cookies.TryGetValue("X-Refresh-Token", out string? refreshToken))
			{
				// Get refresh token
				RefreshToken? foundRefreshToken = await this.tokenRepository.GetByToken(refreshToken);

				if (foundRefreshToken != null)
				{
					// Delete refresh token
					await this.tokenRepository.DeleteByToken(foundRefreshToken.Token);
				}
			}

			HttpContext.Response.Cookies.Delete("X-Access-Token");
			HttpContext.Response.Cookies.Delete("X-Refresh-Token");

			return Ok();
		}
	}
}