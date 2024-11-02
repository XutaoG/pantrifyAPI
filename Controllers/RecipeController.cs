using System.Security.Claims;
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
		private readonly IIngredientRepository ingredientRepository;
		private readonly JwtService jwtService;
		private readonly IMapper mapper;

		public RecipeController(
			IRecipeRepository recipeRepository,
			IIngredientRepository ingredientRepository,
			JwtService jwtService,
			IMapper mapper
		)
		{
			this.recipeRepository = recipeRepository;
			this.ingredientRepository = ingredientRepository;
			this.jwtService = jwtService;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllByUserId(
			[FromQuery] string? name,
			[FromQuery] int? difficulty,
			[FromQuery] int? minDuration,
			[FromQuery] int? maxDuration,
			[FromQuery] string? sortBy,
			[FromQuery] bool? isAscending,
			[FromQuery] int? pageNumber,
			[FromQuery] int? pageSize
		)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				return Unauthorized();
			}

			// Get all recipes belonging to user ID
			List<Recipe> recipes = await this.recipeRepository.GetByUser(
				(int)userId,
				name,
				difficulty,
				minDuration,
				maxDuration,
				sortBy,
				isAscending
			);

			int totalCount = recipes.Count;

			// Pagination
			int skipResult = ((pageNumber ?? 1) - 1) * (pageSize ?? 12);
			recipes = recipes.AsQueryable().Skip(skipResult).Take(pageSize ?? 12).ToList();

			// Map model to Dto
			List<RecipeResponse> recipeResponses = this.mapper.Map<List<RecipeResponse>>(recipes);

			RecipeListResponse response = new RecipeListResponse()
			{
				Recipes = recipeResponses,
				TotalCount = totalCount,
				PageNumber = pageNumber ?? 1,
				PageSize = pageSize ?? 12
			};

			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] AddRecipeRequest addRecipeRequest)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				return Unauthorized();
			}

			// Validate model
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Map Dto to model
			Recipe recipe = this.mapper.Map<Recipe>(addRecipeRequest);

			recipe.UserId = (int)userId;

			// Create recipe
			recipe = await this.recipeRepository.Create(recipe);

			// Map model to Dto
			RecipeResponse response = this.mapper.Map<RecipeResponse>(recipe);

			return CreatedAtAction(nameof(GetbyId), new { id = response.Id }, response);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetbyId([FromRoute] int id)
		{
			(IActionResult res, Recipe? recipe) = await VerifyOwnershipAndExistence(HttpContext.User.Claims.ToList(), id);

			if (recipe == null)
			{
				return res;
			}

			// Map model to Dto
			RecipeResponse response = this.mapper.Map<RecipeResponse>(recipe);

			return Ok(response);
		}

		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> UpdateById([FromRoute] int id, [FromBody] UpdateRecipeRequest updateRecipeRequest)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(HttpContext.User.Claims.ToList());

			// Check if claim user ID exists
			if (userId == null)
			{
				return Unauthorized();
			}

			// Validate model
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Map Dto to model
			Recipe? recipe = this.mapper.Map<Recipe>(updateRecipeRequest);

			// Update model
			recipe = await this.recipeRepository.UpdateById(id, recipe);

			// Check for existence
			if (recipe == null)
			{
				return NotFound();
			}

			// Check if recipe belongs to the matching user
			if (recipe.UserId != userId)
			{
				return Unauthorized();
			}

			// Map model to response Dto
			RecipeResponse response = this.mapper.Map<RecipeResponse>(recipe);

			return Ok(response);
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteById([FromRoute] int id)
		{
			(IActionResult res, Recipe? recipe) = await VerifyOwnershipAndExistence(HttpContext.User.Claims.ToList(), id);

			if (recipe == null)
			{
				return res;
			}

			recipe = await this.recipeRepository.DeleteById(id);

			return NoContent();
		}

		[HttpGet]
		[Route("get-ingredients-availability/{id}")]
		public async Task<IActionResult> GetRecipeIngredientsAvailability([FromRoute] int id)
		{
			(IActionResult res, Recipe? recipe) = await VerifyOwnershipAndExistence(HttpContext.User.Claims.ToList(), id);

			if (recipe == null)
			{
				return res;
			}

			// Initialize response
			List<IngredientAvailabilityResponse> response = [];

			// Check availability of each ingredients
			for (int i = 0; i < recipe.Ingredients.Count; i++)
			{
				Ingredient? ingredient = await this.ingredientRepository.GetByName(recipe.Ingredients[i].Name);

				if (ingredient == null || ingredient.IsAvailable == false)
				{
					response.Add(new IngredientAvailabilityResponse()
					{
						Name = recipe.Ingredients[i].Name,
						Availability = false,
					});
				}
				else
				{
					response.Add(new IngredientAvailabilityResponse()
					{
						Name = recipe.Ingredients[i].Name,
						Availability = true,
					});
				}
			}

			return Ok(response);
		}

		private async Task<(IActionResult, Recipe?)> VerifyOwnershipAndExistence(List<Claim> claims, int id)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(claims);

			// Check if claim user ID exists
			if (userId == null)
			{
				return (Unauthorized(), null);
			}

			// Get ingredient
			Recipe? recipe = await this.recipeRepository.GetById(id);

			// Check for existence
			if (recipe == null)
			{
				return (NotFound(), null);
			}

			// Check if ingredient belongs to the matching user
			if (recipe.UserId != userId)
			{
				return (Unauthorized(), null);
			}

			return (Ok(), recipe);
		}
	}
}