﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Queries;
using TweetBook.Domain;

namespace TweetBook.Services
{
	public interface IPostService
	{
		Task<bool> AddAsync(Post post);
		Task<List<Post>> GetPostsAsync();
		Task<List<Post>> GetPostsAsync(GetAllPostsQuery query = null, PaginationFilter paginationFilter = null);
		Task<Post> GetPostByIdAsync(Guid id);
		Task<bool> UpdatePostAsync(Post postUpdate);
		Task<bool> DeletePostAsync(Guid id);
		Task<bool> UserOwnsPostAsync(Guid postId, string userId);
	}
}
