using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Domain;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
	public class PostsController : Controller
	{
		private readonly IPostService _postService;

		public PostsController(IPostService postService)
		{
			this._postService = postService;
		}

		[HttpGet(APIRoutes.Posts.GetAll)]
		public async Task<IActionResult> GetAll()
		{
			var posts = await _postService.GetPostsAsync();

			if (posts == null || posts.Count == 0)
				return NotFound();

			return Ok(posts);
		}

		[HttpGet(APIRoutes.Posts.Get)]
		public async Task<IActionResult> Get([FromRoute] Guid postId)
		{
			var post = await _postService.GetPostByIdAsync(postId);

			if (post == null)
				return NotFound();

			return Ok(post);
		}

		[Authorize]
		[HttpPost(APIRoutes.Posts.Create)]
		public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
		{
			if (string.IsNullOrEmpty(request.Name))
				return BadRequest();

			var post = new Post { Name = request.Name };

			await _postService.AddAsync(post);

			var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
			var locationUri = baseUrl + "/" + APIRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
			var response = new PostResponse { Id = post.Id, Name = post.Name };

			return Created(locationUri, response);
		}

		[Authorize]
		[HttpPut(APIRoutes.Posts.Update)]
		public async Task<IActionResult> Put([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
		{
			var post = new Post
			{
				Id = postId,
				Name = request.Name
			};

			var updated = await _postService.UpdatePostAsync(post);
			if (updated)
				return Ok(post);

			return NotFound();
		}

		[Authorize]
		[HttpDelete(APIRoutes.Posts.Delete)]
		public async Task<IActionResult> Delete([FromRoute] Guid postId)
		{
			if (postId == null || postId == Guid.Empty)
				return BadRequest();

			var post = await _postService.GetPostByIdAsync(postId);
			var deleted = await _postService.DeletePostAsync(postId);
			if (deleted)
				return Ok(post);

			return NotFound();
		}
	}
}
