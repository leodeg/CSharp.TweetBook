using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TweetBook.Installers
{

	public class MVCInstaller : IInstaller
	{
		public void InstallServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddMvc(options => options.EnableEndpointRouting = false)
				.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

			services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "TweetBook API",
					Description = "A simple example ASP.NET Core REST API",
					Contact = new OpenApiContact
					{
						Name = "LeoDeg",
						Email = string.Empty,
						Url = new Uri("https://github.com/leodeg"),
					}
				});

				var security = new Dictionary<string, IEnumerable<string>>
				{
					{ "Bearer", new string[0]}
				};

				option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description =
						"JWT Authorization header using the Bearer scheme. \r\n\r\n " +
						"Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
						"Example: \"Bearer 12345abcdef\"",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});

				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header,

						},
						new List<string>()
					}
				});

				option.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
			});
		}
	}
}
