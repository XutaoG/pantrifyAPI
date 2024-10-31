using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pantrify.API.Dtos;
using Pantrify.API.Models;
using Pantrify.API.Repositories;
using Pantrify.API.Services;

namespace Pantrify.API.Controllers
{
	[Route("api/ingredient")]
	[ApiController]
	[Authorize]
	public class IngredientController : ControllerBase
	{
		private readonly IIngredientRepository ingredientRepository;
		private readonly JwtService jwtService;
		private readonly IMapper mapper;

		public IngredientController(
			IIngredientRepository ingredientRepository,
			JwtService jwtService,
			IMapper mapper
		)
		{
			this.ingredientRepository = ingredientRepository;
			this.jwtService = jwtService;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllByUserId()
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				// 401
				return Unauthorized();
			}

			// Get all ingredients belonging to user ID
			List<Ingredient> ingredients = await this.ingredientRepository.GetByUser((int)userId);

			// Map model to Dto
			List<IngredientResponse> response = this.mapper.Map<List<IngredientResponse>>(ingredients);

			// 200
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] AddIngredientRequest addIngredientRequest)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				// 401
				return Unauthorized();
			}

			// Validate model
			if (!ModelState.IsValid)
			{
				// 400
				return BadRequest(ModelState);
			}

			// Map Dto to model
			Ingredient ingredient = this.mapper.Map<Ingredient>(addIngredientRequest);
			ingredient.UserId = (int)userId;

			// Create ingredient
			ingredient = await this.ingredientRepository.Create(ingredient);

			// Map model to Dto
			IngredientResponse response = this.mapper.Map<IngredientResponse>(ingredient);

			// 201
			return CreatedAtAction(nameof(GetbyId), new { id = response.Id }, response);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetbyId([FromRoute] int id)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				// 401
				return Unauthorized();
			}

			// Get ingredient
			Ingredient? ingredient = await this.ingredientRepository.GetById(id);

			// Check for existence
			if (ingredient == null)
			{
				// 404
				return NotFound();
			}

			// Check if ingredient belongs to the matching user
			if (ingredient.UserId != userId)
			{
				// 401
				return Unauthorized();
			}

			// Map model to Dto
			IngredientResponse response = this.mapper.Map<IngredientResponse>(ingredient);

			// 200
			return Ok(response);
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> UpdateById([FromRoute] int id, [FromBody] UpdateIngredientRequest updateIngredientDto)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				// 401
				return Unauthorized();
			}

			// Validate model
			if (!ModelState.IsValid)
			{
				// 400
				return BadRequest(ModelState);
			}

			// Map Dto to model
			Ingredient? ingredient = this.mapper.Map<Ingredient>(updateIngredientDto);

			// Update model
			ingredient = await this.ingredientRepository.UpdateById(id, ingredient);

			// Check for existence
			if (ingredient == null)
			{
				// 404
				return NotFound();
			}

			// Check if ingredient belongs to the matching user
			if (ingredient.UserId != userId)
			{
				// 401
				return Unauthorized();
			}

			// Map model to response Dto
			IngredientResponse response = this.mapper.Map<IngredientResponse>(ingredient);

			// 200
			return Ok(response);
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteById([FromRoute] int id)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				// 401
				return Unauthorized();
			}

			// Get ingredient
			Ingredient? ingredient = await this.ingredientRepository.GetById(id);

			// Check for existence
			if (ingredient == null)
			{
				// 404
				return NotFound();
			}

			// Check if ingredient belongs to the matching user
			if (ingredient.UserId != userId)
			{
				// 401
				return Unauthorized();
			}

			ingredient = await this.ingredientRepository.DeleteById(id);

			// 204
			return NoContent();
		}
	}
}