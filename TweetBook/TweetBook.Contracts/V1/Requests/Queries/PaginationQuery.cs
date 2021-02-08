namespace TweetBook.Contracts.V1.Queries
{
	public class PaginationQuery
	{
		public const int MAX_PAGE_SIZE = 1000;

		public int PageNumber { get; set; }
		public int PageSize { get; set; }

		public PaginationQuery()
		{
			PageNumber = 1;
			PageSize = 50;
		}

		public PaginationQuery(int pageNumber, int pageSize)
		{
			PageNumber = pageNumber;
			PageSize = pageSize > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : pageSize;
		}
	}
}
