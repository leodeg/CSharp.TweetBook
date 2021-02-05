using FluentValidation;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.Validators
{
	public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest>
	{
		public UpdatePostRequestValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.MinimumLength(2)
				.MaximumLength(250);
		}
	}
}
