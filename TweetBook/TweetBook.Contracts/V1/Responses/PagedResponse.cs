﻿using System.Collections.Generic;

namespace TweetBook.Contracts.V1
{
	public class PagedResponse<T>
	{
		public IEnumerable<T> Data { get; set; }
		public int? PageNumber { get; set; }
		public int? PageSize { get; set; }
		public string NextPage { get; set; }
		public string PreviousPage { get; set; }

		public PagedResponse() { }

		public PagedResponse(IEnumerable<T> data)
		{
			Data = data;
		}

	}
}
