using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TweetBook.Options;
using TweetBook.Services;

namespace TweetBook.Installers
{
	public class JwtInstaller : IInstaller
	{
		public void InstallServices(IServiceCollection services, IConfiguration configuration)
		{
			var jwtSettings = new JwtSettings();
			configuration.Bind(nameof(JwtSettings), jwtSettings);
			services.AddSingleton(jwtSettings);

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
					ValidateIssuer = false,
					ValidateAudience = false,
					RequireExpirationTime = false,
					ValidateLifetime = true
				};

			});

			services.AddScoped<IIdentityService, IdentityService>();
		}
	}
}
