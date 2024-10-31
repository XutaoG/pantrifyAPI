using AutoMapper;
using Pantrify.API.Dto;
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
		}
	}

}