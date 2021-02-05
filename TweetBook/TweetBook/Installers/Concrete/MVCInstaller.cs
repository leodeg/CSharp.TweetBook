using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TweetBook.Filters;

namespace TweetBook.Installers
{
	public class MVCInstaller : IInstaller
	{
		public void InstallServices(IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddMvc(options =>
				{
					options.EnableEndpointRouting = false;
					options.Filters.Add<ValidationFilter>();
				})
				.AddFluentValidation(configuration =>
					configuration.RegisterValidatorsFromAssemblyContaining<Startup>())
				.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
		}
	}
}
