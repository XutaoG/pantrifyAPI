using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pantrify.API.Dtos;
using Pantrify.API.Models;
using Pantrify.API.Repositories;
using Pantrify.API.Services;

namespace Pantrify.API.Controllers
{
	[Route("api/recipe")]
	[ApiController]
	[Authorize]
	public class RecipeController : ControllerBase
	{
		private readonly IRecipeRepository recipeRepository;
		private readonly JwtService jwtService;
		private readonly IMapper mapper;

		public RecipeController(
			IRecipeRepository recipeRepository,
			JwtService jwtService,
			IMapper mapper
		)
		{
			this.recipeRepository = recipeRepository;
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

			// Get all recipes belonging to user ID
			List<Recipe> recipes = await this.recipeRepository.GetByUser((int)userId);

			// Map model to Dto
			List<RecipeResponse> response = this.mapper.Map<List<RecipeResponse>>(recipes);

			// 200
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] AddRecipeRequest addRecipeRequest)
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
			Recipe recipe = this.mapper.Map<Recipe>(addRecipeRequest);

			// Set recipe instructions steps
			for (int i = 0; i < recipe.Instructions.Count; i++)
			{
				recipe.Instructions[i].Step = i + 1;
			}

			recipe.UserId = (int)userId;

			// Create recipe
			recipe = await this.recipeRepository.Create(recipe);

			// Map model to Dto
			RecipeResponse response = this.mapper.Map<RecipeResponse>(recipe);

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

			// Get recipe
			Recipe? recipe = await this.recipeRepository.GetById(id);

			// Check for existence
			if (recipe == null)
			{
				// 404
				return NotFound();
			}

			// Check if recipe belongs to the matching user
			if (recipe.UserId != userId)
			{
				// 401
				return Unauthorized();
			}

			// Map model to Dto
			RecipeResponse response = this.mapper.Map<RecipeResponse>(recipe);

			// 200
			return Ok(response);
		}
	}
}