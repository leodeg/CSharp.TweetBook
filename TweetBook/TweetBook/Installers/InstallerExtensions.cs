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
				.Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
				.Select(Activator.CreateInstance)
				.Cast<IInstaller>()
				.ToList();

			foreach (var installer in installers)
				installer.InstallServices(services, configuration);
		}
	}
}
