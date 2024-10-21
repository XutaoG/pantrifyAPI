using AutoMapper;
using Pantrify.API.Add.Dto;
using Pantrify.API.Model;
using Pantrify.API.Response.Dto;

namespace Pantrify.API.Mapping
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			// Ingredient
			CreateMap<AddIngredientDto, Ingredient>();
			CreateMap<Ingredient, IngredientResponseDto>();
		}
	}

}