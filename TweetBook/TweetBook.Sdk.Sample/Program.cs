using Refit;
using System;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.Sdk.Sample
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var cachedToken = string.Empty;

			var identityApi = RestService.For<IIdentityApi>("https://localhost:5001");
			var tweetBookApi = RestService.For<ITweetBookApi>("https://localhost:5001", new RefitSettings
			{
				AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
			});

			var registerResponse = await identityApi.RegisterAsync(new UserRegistrationRequest
			{
				Email = "sdktest@tweetbook.com",
				Password = "Sdk_test_0"
			});

			var loginResponse = await identityApi.LoginAsync(new UserLoginRequest
			{
				Email = "sdktest@tweetbook.com",
				Password = "Sdk_test_0"
			});

			if (registerResponse.IsSuccessStatusCode)
			{
				Console.WriteLine("Register response:");
				Console.WriteLine("Token: {0}, Refresh token: {1}", registerResponse.Content.Token, registerResponse.Content.RefreshToken);
			}
			else
			{
				Console.WriteLine(registerResponse.Error);
			}

			if (loginResponse.IsSuccessStatusCode)
			{
				cachedToken = loginResponse.Content.Token;
				Console.WriteLine("Login response:");
				Console.WriteLine("Token: {0}, Refresh token: {1}", loginResponse.Content.Token, loginResponse.Content.RefreshToken);
			}
			else
			{
				Console.WriteLine(loginResponse.Error);
			}

			var allPosts = await tweetBookApi.GetAllAsync();
			var createdPost = await tweetBookApi.CreateAsync(new CreatePostRequest { Name = "Post from SDK: 0" });
			var getPost = await tweetBookApi.GetAsync(createdPost.Content.Id);
			var updatedPost = await tweetBookApi.UpdateAsync(createdPost.Content.Id, new UpdatePostRequest { Name = "Update post from SDK" });
			var deletePost = await tweetBookApi.DeleteAsync(createdPost.Content.Id);
		}
	}
}
