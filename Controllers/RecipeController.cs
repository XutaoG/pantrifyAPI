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
		private readonly CloudinaryService cloudinaryService;
		private readonly IMapper mapper;

		public RecipeController(
			IRecipeRepository recipeRepository,
			IIngredientRepository ingredientRepository,
			JwtService jwtService,
			CloudinaryService cloudinaryService,
			IMapper mapper
		)
		{
			this.recipeRepository = recipeRepository;
			this.ingredientRepository = ingredientRepository;
			this.jwtService = jwtService;
			this.cloudinaryService = cloudinaryService;
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
		public async Task<IActionResult> Create(
			[FromForm] AddRecipeRequest addRecipeRequest
			)
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

			// Upload images to cloudinary
			List<RecipeImage> recipeImages = await this.cloudinaryService.UploadRecipeImages(recipe.Images);

			recipe.Images = recipeImages;
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
		public async Task<IActionResult> UpdateById([FromRoute] int id, [FromForm] UpdateRecipeRequest updateRecipeRequest)
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

			// Upload images to cloudinary
			List<RecipeImage> recipeImages = await this.cloudinaryService.UploadRecipeImages(recipe.Images);

			recipe.Images = recipeImages;

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
				Ingredient? ingredient = await this.ingredientRepository.GetByName(recipe.UserId, recipe.Ingredients[i].Name);

				IngredientAvailabilityResponse availability = new()
				{
					Id = recipe.Ingredients[i].Id,
					Name = recipe.Ingredients[i].Name,
					IngredientType = recipe.Ingredients[i].IngredientType,
					QuantityWhole = recipe.Ingredients[i].QuantityWhole,
					QuantityFraction = recipe.Ingredients[i].QuantityFraction,
					QuantityUnit = recipe.Ingredients[i].QuantityUnit
				};

				if (ingredient == null || ingredient.IsAvailable == false)
				{
					availability.IsAvailable = false;
					availability.IngredientId = null;
				}
				else
				{
					availability.IsAvailable = true;
					availability.IngredientId = ingredient.Id;
				}

				response.Add(availability);
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