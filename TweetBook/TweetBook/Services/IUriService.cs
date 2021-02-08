using System;
using TweetBook.Contracts.V1.Queries;

namespace TweetBook.Services
{
	public interface IUriService
	{
		Uri GetPostUri(string postId);
		Uri GetAllPostsUri(PaginationQuery paginationQuery = null);
	}
}
