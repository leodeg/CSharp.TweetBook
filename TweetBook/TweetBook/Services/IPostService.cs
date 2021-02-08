﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Domain;

namespace TweetBook.Services
{
	public interface IPostService
	{
		Task<bool> AddAsync(Post post);
		Task<List<Post>> GetPostsAsync();
		Task<List<Post>> GetPostsAsync(PaginationFilter paginationFilter);
		Task<Post> GetPostByIdAsync(Guid id);
		Task<bool> UpdatePostAsync(Post postUpdate);
		Task<bool> DeletePostAsync(Guid id);
		Task<bool> UserOwnsPostAsync(Guid postId, string userId);
	}
}
