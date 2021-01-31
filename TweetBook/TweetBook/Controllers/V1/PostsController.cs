using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Domain;

namespace TweetBook.Controllers.V1
{
	public class PostsController : Controller
	{
		private List<Post> _posts;

		public PostsController()
		{
			_posts = new List<Post>();

			for (int i = 0; i < 10; i++)
			{
				_posts.Add(new Post { Id = Guid.NewGuid().ToString() });
			}
		}

		[HttpGet(APIRoutes.Posts.GetAll)]
		public IActionResult GetAll()
		{
			return Ok(_posts);
		}
	}
}
