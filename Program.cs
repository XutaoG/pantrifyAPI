using Microsoft.EntityFrameworkCore;
using Pantrify.API.Data;
using Pantrify.API.Mapping;
using Pantrify.API.Model;
using Pantrify.API.Repositories;
using Pantrify.API.Utils;

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
			builder.Services.AddDbContext<AuthDbcontext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("PantrifyAuthDbConnectionString"));
			});

			// Add repositories to container
			builder.Services.AddScoped<IIngredientRepository, SQLIngredientRepository>();
			builder.Services.AddScoped<IUserRepository, SQLUserRepository>();

			// Add AutoMapper to container
			builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.MapControllers();

			app.Run();
		}
	}
}

