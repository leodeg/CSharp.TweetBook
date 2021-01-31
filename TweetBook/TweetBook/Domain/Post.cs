﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
