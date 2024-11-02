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
		public async Task<IActionResult> GetAllByUserId(
			[FromQuery] string? name,
			[FromQuery] string? ingredientType,
			[FromQuery] bool? isAvailable,
			[FromQuery] bool? isInCart,
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

			// Get all ingredients belonging to user ID
			List<Ingredient> ingredients = await this.ingredientRepository.GetByUser(
				(int)userId,
				name,
				ingredientType,
				isAvailable,
				isInCart,
				sortBy,
				isAscending,
				pageNumber,
				pageSize
			);

			// Map model to Dto
			List<IngredientResponse> response = this.mapper.Map<List<IngredientResponse>>(ingredients);

			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] AddIngredientRequest addIngredientRequest)
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
			Ingredient ingredient = this.mapper.Map<Ingredient>(addIngredientRequest);
			ingredient.UserId = (int)userId;

			// Create ingredient
			ingredient = await this.ingredientRepository.Create(ingredient);

			// Map model to Dto
			IngredientResponse response = this.mapper.Map<IngredientResponse>(ingredient);

			return CreatedAtAction(nameof(GetbyId), new { id = response.Id }, response);
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetbyId([FromRoute] int id)
		{
			(IActionResult res, Ingredient? ingredient) = await VerifyOwnershipAndExistence(HttpContext.User.Claims.ToList(), id);

			if (ingredient == null)
			{
				return res;
			}

			// Map model to Dto
			IngredientResponse response = this.mapper.Map<IngredientResponse>(ingredient);

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
				return Unauthorized();
			}

			// Validate model
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Map Dto to model
			Ingredient? ingredient = this.mapper.Map<Ingredient>(updateIngredientDto);

			// Update model
			ingredient = await this.ingredientRepository.UpdateById(id, ingredient);

			// Check for existence
			if (ingredient == null)
			{
				return NotFound();
			}

			// Check if ingredient belongs to the matching user
			if (ingredient.UserId != userId)
			{
				return Unauthorized();
			}

			// Map model to response Dto
			IngredientResponse response = this.mapper.Map<IngredientResponse>(ingredient);

			return Ok(response);
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteById([FromRoute] int id)
		{
			(IActionResult res, Ingredient? ingredient) = await VerifyOwnershipAndExistence(HttpContext.User.Claims.ToList(), id);

			if (ingredient == null)
			{
				return res;
			}

			ingredient = await this.ingredientRepository.DeleteById(id);

			return NoContent();
		}

		[HttpPost]
		[Route("move-to-cart/{id}")]
		public async Task<IActionResult> MoveToCart([FromRoute] int id)
		{
			(IActionResult res, Ingredient? ingredient) = await VerifyOwnershipAndExistence(HttpContext.User.Claims.ToList(), id);

			if (ingredient == null)
			{
				return res;
			}

			// Update availability
			ingredient.IsAvailable = false;
			ingredient.IsInCart = true;

			// Update model
			ingredient = await this.ingredientRepository.UpdateById(id, ingredient);

			return Ok(ingredient);
		}

		[HttpPost]
		[Route("move-to-inventory/{id}")]
		public async Task<IActionResult> MoveToInventory([FromRoute] int id)
		{
			(IActionResult res, Ingredient? ingredient) = await VerifyOwnershipAndExistence(HttpContext.User.Claims.ToList(), id);

			if (ingredient == null)
			{
				return res;
			}

			// Update availability
			ingredient.IsAvailable = true;
			ingredient.IsInCart = false;

			// Update model
			ingredient = await this.ingredientRepository.UpdateById(id, ingredient);

			return Ok(ingredient);
		}

		[HttpPost]
		[Route("add-recipe-ingredient-to-cart")]
		public async Task<IActionResult> AddRecipeIngredientToCart([FromBody] AddRecipeIngredientRequest addRecipeIngredientRequest)
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

			// Check if recipe ingredient exists
			Ingredient? ingredient = await this.ingredientRepository.GetByName(addRecipeIngredientRequest.Name);
			if (ingredient != null)
			{
				return await this.MoveToCart(ingredient.Id);
			}

			// Map Dto to model
			RecipeIngredient recipeIngredient = this.mapper.Map<RecipeIngredient>(addRecipeIngredientRequest);

			// Map recipe ingredient to ingredient
			ingredient = this.mapper.Map<Ingredient>(recipeIngredient);
			ingredient.IsAvailable = false;
			ingredient.IsInCart = true;
			ingredient.UserId = (int)userId;

			ingredient = await this.ingredientRepository.Create(ingredient);

			return Ok(ingredient);
		}

		[HttpPost]
		[Route("add-recipe-ingredient-to-inventory")]
		public async Task<IActionResult> AddRecipeIngredientToInventory([FromBody] AddRecipeIngredientRequest addRecipeIngredientRequest)
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

			// Check if recipe ingredient exists
			Ingredient? ingredient = await this.ingredientRepository.GetByName(addRecipeIngredientRequest.Name);
			if (ingredient != null)
			{
				return await this.MoveToInventory(ingredient.Id);
			}

			// Map Dto to model
			RecipeIngredient recipeIngredient = this.mapper.Map<RecipeIngredient>(addRecipeIngredientRequest);

			// Map recipe ingredient to ingredient
			ingredient = this.mapper.Map<Ingredient>(recipeIngredient);
			ingredient.IsAvailable = true;
			ingredient.IsInCart = false;
			ingredient.UserId = (int)userId;

			ingredient = await this.ingredientRepository.Create(ingredient);

			return Ok(ingredient);
		}

		private async Task<(IActionResult, Ingredient?)> VerifyOwnershipAndExistence(List<Claim> claims, int id)
		{
			int? userId = this.jwtService.GetUserIdFromClaims(claims);

			// Check if claim user ID exists
			if (userId == null)
			{
				return (Unauthorized(), null);
			}

			// Get ingredient
			Ingredient? ingredient = await this.ingredientRepository.GetById(id);

			// Check for existence
			if (ingredient == null)
			{
				return (NotFound(), null);
			}

			// Check if ingredient belongs to the matching user
			if (ingredient.UserId != userId)
			{
				return (Unauthorized(), null);
			}

			return (Ok(), ingredient);
		}
	}
}