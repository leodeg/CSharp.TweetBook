using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;
using TweetBook.Contracts.HealthCheck;
using TweetBook.Installers;

namespace TweetBook
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.InstallServicesAssembly(Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHealthChecks("/health", new HealthCheckOptions
			{
				ResponseWriter = async (context, report) =>
				{
					context.Response.ContentType = "application/json";
					var response = new HealthCheckResponse
					{
						Status = report.Status.ToString(),
						HealthChecks = report.Entries.Select(x => new HealthCheck
						{
							Component = x.Key,
							Status = x.Value.Status.ToString(),
							Description = x.Value.Description
						}),
						Duration = report.TotalDuration
					};

					await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
				}
			});

			var swaggerOptions = new TweetBook.Options.SwaggerOptions();
			Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

			app.UseSwagger(option =>
			{
				option.RouteTemplate = swaggerOptions.JsonRoute;
			});

			app.UseSwaggerUI(option =>
			{
				option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
			});

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
