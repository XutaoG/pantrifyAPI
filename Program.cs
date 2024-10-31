using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pantrify.API.Data;
using Pantrify.API.Mapping;
using Pantrify.API.Repositories;
using Pantrify.API.Services;

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
			builder.Services.AddScoped<ITokenRepository, SQLTokenRepository>();

			// Add AutoMapper to container
			builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

			// Add PasswordHasherService to container
			builder.Services.AddSingleton(typeof(PasswordHashService));

			// Add JwtService to container
			builder.Services.AddTransient(typeof(JwtService));

			// Add JWT authentication
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("secret_key") ?? "")
					),
					ValidateLifetime = true,
					ValidateAudience = false,
					ValidateIssuer = false,
					ClockSkew = TimeSpan.Zero
				};
			});

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			// Add authentication and authorization middleware
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}

