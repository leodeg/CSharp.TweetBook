using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;

namespace TweetBook
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			using (var serviceScope = host.Services.CreateScope())
			{
				//var dbContext = serviceScope.ServiceProvider
				//	.GetRequiredService<DataContext>();

				var roleManager = serviceScope.ServiceProvider
					.GetRequiredService<RoleManager<IdentityRole>>();

				if (!await roleManager.RoleExistsAsync(Roles.Admin))
				{
					var adminRole = new IdentityRole(Roles.Admin);
					await roleManager.CreateAsync(adminRole);
				}

				if (!await roleManager.RoleExistsAsync(Roles.NormalUser))
				{
					var normalUserRole = new IdentityRole(Roles.NormalUser);
					await roleManager.CreateAsync(normalUserRole);
				}
			}

			await host.RunAsync();
		}

		public static IWebHostBuilder CreateHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
	}
}
