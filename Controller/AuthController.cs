using Microsoft.AspNetCore.Mvc;
using Pantrify.API.Dto.Auth;
using Pantrify.API.Model;
using Pantrify.API.Repositories;
using Pantrify.API.Utils;

namespace Pantrify.API.Controller
{
	[Route("auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IUserRepository userRepository;

		public AuthController(
			IUserRepository userRepository // Inject UserRepository
		)
		{
			this.userRepository = userRepository;
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
				PasswordHash = PasswordHasherService.HashPassword(signUpRequest.Password)
			};

			user = await this.userRepository.Create(user);

			if (user == null)
			{
				ModelState.AddModelError("Email", "Email already used");
			}
			else
			{
				// 200
				return Ok();
			}

			// 400
			return BadRequest(ModelState);
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

			// 200
			return Ok();
		}
	}
}