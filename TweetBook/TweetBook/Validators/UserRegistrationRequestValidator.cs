using FluentValidation;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.Validators
{
	public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
	{
		public UserRegistrationRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress();
		}
	}
}
