using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Domain;
using Xunit;

namespace TweetBook.IntegrationTests
{
	public class PostControllerTests : IntegrationTest
	{
		[Fact]
		public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
		{
			// Arrange
			await AuthenticateAsync();

			// Act
			var response = await TestClient.GetAsync(APIRoutes.Posts.GetAll);

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			(await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();
		}
	}
}
