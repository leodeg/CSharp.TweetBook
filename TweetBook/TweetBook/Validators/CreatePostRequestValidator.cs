﻿using FluentValidation;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.Validators
{
	public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
	{
		public CreatePostRequestValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.MinimumLength(2)
				.MaximumLength(250);
		}
	}
}
