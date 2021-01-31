using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Installers
{

	public class MVCInstaller : IInstaller
	{
		public void InstallServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddMvc();
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
			});
		}
	}
}
