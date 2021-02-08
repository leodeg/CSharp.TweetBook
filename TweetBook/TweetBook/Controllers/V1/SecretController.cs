using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Filters;

namespace TweetBook.Controllers.V1
{
	[ApiKeyAuth]
	public class SecretController : ControllerBase
	{
		[HttpGet("secret")]
		public async Task<IActionResult> GetSecret ()
		{
			return Ok("No secrets");
		}
	}
}
