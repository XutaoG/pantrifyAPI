using Microsoft.AspNetCore.Mvc;
using Pantrify.API.Add.Dto;
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
		public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
		{
			if (!ModelState.IsValid)
			{
				// 400
				return BadRequest(ModelState);
			}

			User? user = new User()
			{
				Email = signUpDto.Email,
				FirstName = signUpDto.FirstName,
				LastName = signUpDto.LastName,
				PasswordHash = PasswordHasherService.HashPassword(signUpDto.Password)
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
	}
}