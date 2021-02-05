using FluentValidation;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.Validators
{
	public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
	{
		public UserLoginRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress();
		}
	}
}
