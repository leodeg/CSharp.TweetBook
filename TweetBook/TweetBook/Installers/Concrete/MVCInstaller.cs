using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TweetBook.Filters;
using TweetBook.Services;

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

			services.AddSingleton<IUriService, UriService>(provider =>
			{
				var accessor = provider.GetRequiredService<IHttpContextAccessor>();
				var request = accessor.HttpContext.Request;
				var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
				return new UriService(absoluteUri);
			});
		}
	}
}
