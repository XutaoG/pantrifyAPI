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

			// Add CORS
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("allowedOrigins", policy =>
				{
					policy.WithOrigins("http://localhost:3000")
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
				});
			});

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

			// Add repositories to container
			builder.Services.AddScoped<IIngredientRepository, SQLIngredientRepository>();
			builder.Services.AddScoped<IUserRepository, SQLUserRepository>();
			builder.Services.AddScoped<ITokenRepository, SQLTokenRepository>();
			builder.Services.AddScoped<IRecipeRepository, SQLRecipeRepository>();

			// Add AutoMapper to container
			builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

			// Add services to container
			builder.Services.AddSingleton(typeof(PasswordHashService));
			builder.Services.AddTransient(typeof(JwtService));
			builder.Services.AddSingleton(typeof(CloudinaryService));

			// Add JWT authentication
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.SaveToken = true;
				// Set JWT validation
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

				options.Events = new JwtBearerEvents
				{
					// Extract token from cookie header
					OnMessageReceived = context =>
					{
						if (context.Request.Cookies.ContainsKey("X-Access-Token"))
						{
							context.Token = context.Request.Cookies["X-Access-Token"];
						}

						return Task.CompletedTask;
					}
				};
			});

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseCors("allowedOrigins");

			// Add authentication and authorization middleware
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}

