using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TweetBook.Authorization;
using TweetBook.Contracts.V1;
using TweetBook.Options;
using TweetBook.Services;

namespace TweetBook.Installers
{
	public class AuthInstaller : IInstaller
	{
		public void InstallServices(IServiceCollection services, IConfiguration configuration)
		{
			var jwtSettings = new JwtSettings();
			configuration.Bind(nameof(JwtSettings), jwtSettings);

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
				ValidateIssuer = false,
				ValidateAudience = false,
				RequireExpirationTime = false,
				ValidateLifetime = true
			};

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.TokenValidationParameters = tokenValidationParameters;

			});

			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policy.MustWorkForCompany,
					policy => policy.AddRequirements(
						new WorksForCompanyRequirement("company.com")));
			});

			services.AddSingleton(jwtSettings);
			services.AddSingleton(tokenValidationParameters);
			services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();
			services.AddScoped<IIdentityService, IdentityService>();
		}
	}
}
