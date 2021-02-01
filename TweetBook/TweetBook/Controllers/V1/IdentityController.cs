using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;
using TweetBook.Services;

namespace TweetBook.Controllers.V1
{
	public class IdentityController : Controller
	{
		private readonly IIdentityService _identityService;

		public IdentityController(IIdentityService identityService)
		{
			this._identityService = identityService;
		}

		[HttpPost(APIRoutes.Identity.Register)]
		public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new AuthFailedResponse
				{
					Errors = ModelState.Values.SelectMany(
						state => state.Errors.Select(
							error => error.ErrorMessage))
				});
			}

			var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);

			if (!authResponse.Success)
			{
				return BadRequest(new AuthFailedResponse
				{
					Errors = authResponse.Errors
				});
			}

			return Ok(new AuthSuccessResponse
			{
				Token = authResponse.Token
			});
		}

		[HttpPost(APIRoutes.Identity.Login)]
		public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new AuthFailedResponse
				{
					Errors = ModelState.Values.SelectMany(
						state => state.Errors.Select(
							error => error.ErrorMessage))
				});
			}

			var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

			if (!authResponse.Success)
			{
				return BadRequest(new AuthFailedResponse
				{
					Errors = authResponse.Errors
				});
			}

			return Ok(new AuthSuccessResponse
			{
				Token = authResponse.Token
			});
		}
	}
}
