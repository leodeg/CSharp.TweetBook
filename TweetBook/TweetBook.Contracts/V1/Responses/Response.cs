﻿namespace TweetBook.Contracts.V1
{
	public class Response<T>
	{
		public T Data { get; set; }

		public Response() { }

		public Response(T response)
		{
			Data = response;
		}


	}
}
