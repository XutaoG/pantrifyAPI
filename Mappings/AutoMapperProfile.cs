using AutoMapper;
using Pantrify.API.Dtos;
using Pantrify.API.Models;

namespace Pantrify.API.Mapping
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			// Ingredient
			CreateMap<AddIngredientRequest, Ingredient>();
			CreateMap<UpdateIngredientRequest, Ingredient>();
			CreateMap<Ingredient, IngredientResponse>();

			// Recipe Ingredient
			CreateMap<AddRecipeIngredientRequest, RecipeIngredient>();
			CreateMap<RecipeIngredient, RecipeIngredientResponse>();

			// Recipe Instruction
			CreateMap<AddRecipeInstructionRequest, RecipeInstruction>();
			CreateMap<RecipeInstruction, RecipeInstructionResponse>();

			// Recipe
			CreateMap<AddRecipeRequest, Recipe>();
			CreateMap<Recipe, RecipeResponse>();
		}
	}

}