using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Data;
using TweetBook.Domain;

namespace TweetBook.Services
{
	public class PostService : IPostService
	{
		private readonly DataContext _data;

		public PostService(DataContext data)
		{
			_data = data;
		}

		public async Task<bool> AddAsync(Post post)
		{
			await _data.Posts.AddAsync(post);
			var created = await _data.SaveChangesAsync();
			return created > 0;
		}

		public async Task<List<Post>> GetPostsAsync()
		{
			return await _data.Posts.AsNoTracking().ToListAsync();
		}

		public async Task<Post> GetPostByIdAsync(Guid id)
		{
			return await _data.Posts.AsNoTracking().SingleOrDefaultAsync(post => post.Id == id);
		}

		public async Task<bool> UpdatePostAsync(Post postUpdate)
		{
			_data.Posts.Update(postUpdate);
			var updated = await _data.SaveChangesAsync();
			return updated > 0;
		}

		public async Task<bool> DeletePostAsync(Guid id)
		{
			var post = await GetPostByIdAsync(id);
			_data.Posts.Remove(post);
			var deleted = await _data.SaveChangesAsync();
			return deleted > 0;
		}

		public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
		{
			var post = await _data.Posts.AsNoTracking().SingleOrDefaultAsync(post => post.Id == postId);

			if (post == null)
				return false;

			if (post.UserId != userId)
				return false;

			return true;
		}
	}
}
