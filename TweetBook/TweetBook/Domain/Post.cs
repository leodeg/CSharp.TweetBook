using System;
using System.ComponentModel.DataAnnotations;

namespace TweetBook.Domain
{
	public class Post
	{
		[Key]
		public Guid Id { get; set; }

		[MaxLength(250)]
		public string Name { get; set; }
	}
}
