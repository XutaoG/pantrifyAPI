using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Pantrify.API.Models;

namespace Pantrify.API.Services
{
	public class CloudinaryService
	{
		private readonly Cloudinary cloudinary;

		public CloudinaryService(
			IConfiguration configuration
		)
		{
			// Read secrets from config
			string cloudinaryApiKey = configuration.GetValue<string>("cloudinary_api_key")!;
			string cloudinaryApiSecret = configuration.GetValue<string>("cloudinary_api_secret")!;
			string cloudName = configuration.GetValue<string>("cloudinary_cloud_name")!;

			this.cloudinary = new Cloudinary($"cloudinary://{cloudinaryApiKey}:{cloudinaryApiSecret}@{cloudName}");
		}

		public async Task<List<RecipeImage>> UploadRecipeImages(List<RecipeImage> recipeImages)
		{
			for (int i = 0; i < recipeImages.Count; i++)
			{
				// Upload to cloudinary
				UploadResult result = await this.cloudinary.UploadAsync(new ImageUploadParams()
				{
					File = new FileDescription(recipeImages[i].File.FileName, recipeImages[i].File.OpenReadStream())
				});

				// Transform image
				string url = this.cloudinary.Api.UrlImgUp.Transform(new Transformation()
					.Width(1200).Chain()
					.Quality("auto").Chain()
					.FetchFormat("auto")
				).BuildUrl((string)result.JsonObj["public_id"]!);

				recipeImages[i].Path = url;
			}

			return recipeImages;
		}
	}
}