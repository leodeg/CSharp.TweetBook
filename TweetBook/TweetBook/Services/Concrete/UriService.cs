using Microsoft.AspNetCore.WebUtilities;
using System;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Queries;

namespace TweetBook.Services
{
	public class UriService : IUriService
	{
		private readonly string _baseUri;

		public UriService(string baseUri)
		{
			this._baseUri = baseUri;
		}

		public Uri GetAllPostsUri(PaginationQuery paginationQuery = null)
		{
			var uri = new Uri(_baseUri);

			if (paginationQuery == null)
				return uri;

			var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber",
				paginationQuery.PageNumber.ToString());

			modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageSize",
				paginationQuery.PageSize.ToString());

			return new Uri(modifiedUri);
		}

		public Uri GetPostUri(string postId)
		{
			return new Uri(_baseUri + APIRoutes.Posts.Get.Replace("{postId}", postId));
		}
	}
}
