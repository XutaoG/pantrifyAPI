using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Pantrify.API.Add.Dto;
using Pantrify.API.Model;
using Pantrify.API.Repository;
using Pantrify.API.Response.Dto;

namespace Pantrify.API.Controller
{
	[Route("api/ingredient")]
	[ApiController]
	public class IngredientController : ControllerBase
	{
		private readonly IIngredientRepository ingredientRepository;
		private readonly IMapper mapper;

		public IngredientController(
			IIngredientRepository ingredientRepository, // Inject repository
			IMapper mapper // Inject mapper
		)
		{
			this.ingredientRepository = ingredientRepository;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			// Get all ingredients
			List<Ingredient> ingredients = await this.ingredientRepository.GetAll();

			// Map model to Dto
			List<IngredientResponseDto> response = this.mapper.Map<List<IngredientResponseDto>>(ingredients);

			// 200
			return Ok(response);
		}

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
			IngredientResponseDto response = this.mapper.Map<IngredientResponseDto>(ingredient);

			// 200
			return Ok(response);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] AddIngredientDto addIngredientDto)
		{
			// Validate model
			if (!ModelState.IsValid)
			{
				// 400
				return BadRequest(ModelState);
			}

			// Map Dto to model
			Ingredient ingredient = this.mapper.Map<Ingredient>(addIngredientDto);

			// Create ingredient
			ingredient = await this.ingredientRepository.Create(ingredient);

			// Map model to Dto
			IngredientResponseDto response = this.mapper.Map<IngredientResponseDto>(ingredient);

			// 201
			return CreatedAtAction(nameof(GetbyId), new { id = response.Id }, response);
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
			IngredientResponseDto response = this.mapper.Map<IngredientResponseDto>(ingredient);

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