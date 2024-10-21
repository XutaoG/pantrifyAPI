using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;

namespace Pantrify
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add controllers to container
			builder.Services.AddControllers();

			// Add services to container
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// Add DbContext to container
			builder.Services.AddDbContext<PantrifyDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("PantrifyDbConnectionString"));
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.Run();
		}
	}
}

