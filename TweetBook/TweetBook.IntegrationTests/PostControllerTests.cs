using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Data;
using TweetBook.Domain;
using Xunit;
using Xunit.Sdk;

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
