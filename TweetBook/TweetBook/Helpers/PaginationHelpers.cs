using System.Collections.Generic;
using System.Linq;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Queries;
using TweetBook.Domain;
using TweetBook.Services;

namespace TweetBook.Helpers
{
	public static class PaginationHelpers
	{
		internal static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, PaginationFilter paginationFilter, List<T> postResponses)
		{
			var nextPage = paginationFilter.PageNumber >= 1 ?
				uriService.GetAllPostsUri(new PaginationQuery(paginationFilter.PageNumber + 1, paginationFilter.PageSize)).ToString()
				: null;

			var previousPage = paginationFilter.PageNumber - 1 >= 1 ?
				uriService.GetAllPostsUri(new PaginationQuery(paginationFilter.PageNumber - 1, paginationFilter.PageSize)).ToString()
				: null;

			return new PagedResponse<T>
			{
				Data = postResponses,
				PageNumber = paginationFilter.PageNumber >= 1 ? paginationFilter.PageNumber : (int?)null,
				PageSize = paginationFilter.PageSize >= 1 ? paginationFilter.PageSize : (int?)null,
				NextPage = postResponses.Any() ? nextPage : null,
				PreviousPage = previousPage
			};
		}
	}
}
