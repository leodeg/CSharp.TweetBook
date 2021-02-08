using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TweetBook.Authorization;
using TweetBook.Contracts.V1;
using TweetBook.Data;
using TweetBook.HealthChecks;
using TweetBook.Options;
using TweetBook.Services;

namespace TweetBook.Installers
{
	public class HealthChecksInstaller : IInstaller
	{
		public void InstallServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddHealthChecks()
				.AddDbContextCheck<DataContext>()
				.AddCheck<RedisHealthCheck>("Redis");
		}
	}
}
