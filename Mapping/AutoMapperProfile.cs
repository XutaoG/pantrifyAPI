using AutoMapper;
using Pantrify.API.Add.Dto;
using Pantrify.API.Dto.Ingredient;
using Pantrify.API.Models;

namespace Pantrify.API.Mapping
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			// Ingredient
			CreateMap<AddIngredientRequest, Ingredient>();
			CreateMap<UpdateIngredientDto, Ingredient>();
			CreateMap<Ingredient, IngredientResponse>();
		}
	}

}