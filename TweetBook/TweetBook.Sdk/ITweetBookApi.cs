using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;

namespace TweetBook.Sdk
{
	[Headers("Authorization: Bearer")]
	public interface ITweetBookApi
	{
		[Get("/api/v1/posts")]
		Task<ApiResponse<List<PostResponse>>> GetAllAsync();

		[Get("/api/v1/posts/{postId}")]
		Task<ApiResponse<List<PostResponse>>> GetAsync(Guid postId);

		[Post("/api/v1/posts")]
		Task<ApiResponse<PostResponse>> CreateAsync([Body] CreatePostRequest postRequest);

		[Put("/api/v1/posts/{postId}")]
		Task<ApiResponse<PostResponse>> UpdateAsync(Guid postId, [Body] UpdatePostRequest postRequest);

		[Delete("/api/v1/posts/{postId}")]
		Task<ApiResponse<string>> DeleteAsync(Guid postId);
	}
}
