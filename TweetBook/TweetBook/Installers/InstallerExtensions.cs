using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace TweetBook.Installers
{
	public static class InstallerExtensions
	{
		public static void InstallServicesAssembly(this IServiceCollection services, IConfiguration configuration)
		{
			var installers = typeof(Startup).Assembly.ExportedTypes
				.Where(installer => typeof(IInstaller).IsAssignableFrom(installer)
					&& !installer.IsInterface
					&& !installer.IsAbstract)
				.Select(Activator.CreateInstance)
				.Cast<IInstaller>()
				.ToList();

			foreach (var installer in installers)
				installer.InstallServices(services, configuration);
		}
	}
}
