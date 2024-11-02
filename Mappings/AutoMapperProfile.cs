using AutoMapper;
using Pantrify.API.Dtos;
using Pantrify.API.Models;
using Pantrify.API.Services;

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

			CreateMap<RecipeIngredient, Ingredient>();

			// Recipe Ingredient
			CreateMap<AddRecipeIngredientRequest, RecipeIngredient>();
			CreateMap<UpdateRecipeIngredientRequest, RecipeIngredient>();
			CreateMap<RecipeIngredient, RecipeIngredientResponse>();

			// Recipe Instruction
			CreateMap<RecipeInstruction, RecipeInstructionResponse>();

			// Recipe Image
			CreateMap<RecipeImage, RecipeImageResponse>();

			// Recipe
			CreateMap<AddRecipeRequest, Recipe>().ForMember(dest => dest.Instructions,
				opt => opt.MapFrom(src => src.Instructions.Select((ins, i) => new RecipeInstruction()
				{
					Step = i + 1,
					Instruction = ins,
				})))
				.ForMember(dest => dest.Images,
				opt => opt.MapFrom(src => src.Images.Select((img, i) => new RecipeImage()
				{
					File = img,
					Order = i + 1,
				})));
			CreateMap<UpdateRecipeRequest, Recipe>().ForMember(dest => dest.Instructions,
				opt => opt.MapFrom(src => src.Instructions.Select((ins, i) => new RecipeInstruction()
				{
					Step = i + 1,
					Instruction = ins
				})));
			CreateMap<Recipe, RecipeResponse>();
		}
	}
}