using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Data;

namespace TweetBook.IntegrationTests
{
	public class IntegrationTest : IDisposable
	{
		protected readonly HttpClient TestClient;
		private readonly IServiceProvider serviceProvider;

		public IntegrationTest()
		{
			var appFactory = new WebApplicationFactory<Startup>()
				.WithWebHostBuilder(builder =>
				{
					builder.ConfigureServices(services =>
					{
						services.RemoveAll(typeof(DataContext));
						services.AddDbContext<DataContext>(options =>
							options.UseInMemoryDatabase("TweetBook-TestDB"));
					});
				});

			serviceProvider = appFactory.Services;
			TestClient = appFactory.CreateClient();
		}

		protected async Task AuthenticateAsync()
		{
			TestClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", await GetJwtAsync());
		}

		private async Task<string> GetJwtAsync()
		{
			var user = new UserRegistrationRequest
			{
				Email = "test@integration.com",
				Password = "Test_01234"
			};

			var response = await TestClient.PostAsJsonAsync(APIRoutes.Identity.Register, user);
			response.EnsureSuccessStatusCode();

			var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
			return registrationResponse.Token;
		}

		protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
		{
			var response = await TestClient.PostAsJsonAsync(APIRoutes.Posts.Create, request);
			return await response.Content.ReadAsAsync<PostResponse>();
		}

		public void Dispose()
		{
			using var serviceScope = serviceProvider.CreateScope();
			var context = serviceScope.ServiceProvider.GetService<DataContext>();
			context.Database.EnsureDeleted();
		}
	}
}
