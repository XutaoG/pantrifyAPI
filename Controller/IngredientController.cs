using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pantrify.API.Add.Dto;
using Pantrify.API.Dto.Ingredient;
using Pantrify.API.Models;
using Pantrify.API.Repositories;
using Pantrify.API.Services;

namespace Pantrify.API.Controller
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

			if (userId == null)
			{
				ModelState.AddModelError("Token", "Invalid token");

				// 400
				return BadRequest(ModelState);
			}

			// Get all ingredients belonging to user ID
			List<Ingredient> ingredients = await this.ingredientRepository.GetIngredientsByUser((int)userId);

			// Map model to Dto
			List<IngredientResponse> response = this.mapper.Map<List<IngredientResponse>>(ingredients);

			// 200
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] AddIngredientRequest addIngredientRequest)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			if (userId == null)
			{
				ModelState.AddModelError("Token", "Invalid token");

				// 400
				return BadRequest(ModelState);
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

		// [HttpGet]
		// public async Task<IActionResult> GetAll()
		// {
		// 	// Get all ingredients
		// 	List<Ingredient> ingredients = await this.ingredientRepository.GetAll();

		// 	// Map model to Dto
		// 	List<IngredientResponseDto> response = this.mapper.Map<List<IngredientResponseDto>>(ingredients);

		// 	// 200
		// 	return Ok(response);
		// }

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetbyId([FromRoute] int id)
		{
			// Get ingredient
			Ingredient? ingredient = await this.ingredientRepository.GetbyId(id);

			// Check for existence
			if (ingredient == null)
			{
				// 404
				return NotFound();
			}

			// Map model to Dto
			IngredientResponse response = this.mapper.Map<IngredientResponse>(ingredient);

			// 200
			return Ok(response);
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> UpdateById([FromRoute] int id, [FromBody] UpdateIngredientDto updateIngredientDto)
		{
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

			// Map model to response Dto
			IngredientResponse response = this.mapper.Map<IngredientResponse>(ingredient);

			// 200
			return Ok(response);
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteById([FromRoute] int id)
		{
			Ingredient? ingredient = await this.ingredientRepository.DeleteById(id);

			// Check for existence
			if (ingredient == null)
			{
				// 404
				return NotFound();
			}

			// 204
			return NoContent();
		}
	}
}