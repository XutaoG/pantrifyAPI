namespace Pantrify.API.Dtos
{
	public class RecipeResponse
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public int Duration { get; set; }

		public int Difficulty { get; set; }

		public int NumServings { get; set; }

		public DateTime DateAdded { get; set; }

		public DateTime DateModified { get; set; }

		public List<RecipeIngredientResponse> Ingredients { get; set; } = null!;

		public List<RecipeInstructionResponse> Instructions { get; set; } = null!;

		public List<RecipeImageResponse> Images { get; set; } = null!;
	}

	public class RecipeIngredientResponse
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public string IngredientType { get; set; } = null!;

		public int? Quantity { get; set; }

		public string QuantityUnit { get; set; } = null!;
	}

	public class RecipeInstructionResponse
	{
		public int Id { get; set; }

		public int Step { get; set; }

		public string Instruction { get; set; } = null!;
	}

	public class RecipeImageResponse
	{
		public int Id { get; set; }

		public string Path { get; set; } = null!;

		public int Order { get; set; }
	}
}