using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Cache;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Queries;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;
using TweetBook.Extensions;
using TweetBook.Helpers;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class PostsController : Controller
	{
		private readonly IPostService _postService;
		private readonly IUriService _uriService;

		public PostsController(IPostService postService, IUriService uriService)
		{
			this._postService = postService;
			this._uriService = uriService;
		}

		[Cached(600)]
		[HttpGet(APIRoutes.Posts.GetAll)]
		public async Task<IActionResult> GetAll([FromQuery] GetAllPostsQuery query, [FromQuery] PaginationQuery paginationQuery)
		{
			var paginationFilter = new PaginationFilter
			{
				PageNumber = paginationQuery.PageNumber,
				PageSize = paginationQuery.PageSize
			};

			var posts = await _postService.GetPostsAsync(query, paginationFilter);

			var postResponses = new List<PostResponse>();
			foreach (var post in posts)
				postResponses.Add(new PostResponse { Id = post.Id, Name = post.Name });

			if (paginationFilter == null || paginationFilter.PageNumber < 1 || paginationFilter.PageSize < 1)
				return Ok(new PagedResponse<PostResponse>(postResponses));

			var paginatedResponse = PaginationHelpers.CreatePaginatedResponse<PostResponse>(_uriService, paginationFilter, postResponses);
			return Ok(paginatedResponse);
		}

		[Cached(600)]
		[HttpGet(APIRoutes.Posts.Get)]
		public async Task<IActionResult> Get([FromRoute] Guid postId)
		{
			if (postId == null || postId == Guid.Empty)
				return BadRequest(new { Error = $"Request is empty!" });

			var post = await _postService.GetPostByIdAsync(postId);

			if (post == null)
				return NotFound();

			return Ok(new Response<PostResponse>(new PostResponse { Id = post.Id, Name = post.Name }));
		}

		[HttpPost(APIRoutes.Posts.Create)]
		public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.Values.SelectMany(state => state.Errors));

			var post = new Post { Name = request.Name, UserId = HttpContext.GetUserId() };

			await _postService.AddAsync(post);

			var response = new PostResponse { Id = post.Id, Name = post.Name };
			var locationUri = _uriService.GetPostUri(response.Id.ToString());

			return Created(locationUri, new Response<PostResponse>(response));
		}

		[HttpPut(APIRoutes.Posts.Update)]
		public async Task<IActionResult> Put([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
		{
			if (postId == null || postId == Guid.Empty)
				return BadRequest(new { Error = $"Request is empty!" });

			var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

			if (!userOwnsPost)
				return BadRequest(new { Error = "You do not own this post!" });

			var post = await _postService.GetPostByIdAsync(postId);
			post.Name = request.Name;

			var updated = await _postService.UpdatePostAsync(post);
			if (updated)
			{
				var response = new PostResponse { Id = post.Id, Name = post.Name };
				return Ok(new Response<PostResponse>(response));
			}

			return NotFound();
		}

		[HttpDelete(APIRoutes.Posts.Delete)]
		public async Task<IActionResult> Delete([FromRoute] Guid postId)
		{
			if (postId == null || postId == Guid.Empty)
				return BadRequest(new { Error = $"Request is empty!" });

			var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

			if (!userOwnsPost)
				return BadRequest(new { Error = "You do not own this post!" });

			if (postId == null || postId == Guid.Empty)
				return BadRequest();

			var post = await _postService.GetPostByIdAsync(postId);
			var deleted = await _postService.DeletePostAsync(postId);
			if (deleted)
				return Ok(new Response<PostResponse>(new PostResponse { Id = post.Id, Name = post.Name }));

			return NotFound();
		}
	}
}
