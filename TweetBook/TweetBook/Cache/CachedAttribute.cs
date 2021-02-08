using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Services;

namespace TweetBook.Cache
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class CachedAttribute : Attribute, IAsyncActionFilter
	{
		private readonly int _timeToLiveSeconds;

		public CachedAttribute(int timeToLiveSeconds)
		{
			this._timeToLiveSeconds = timeToLiveSeconds;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<RedisCacheSettings>();
			if (!cacheSettings.Enabled)
			{
				await next();
				return;
			}

			var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
			var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
			var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

			if (!string.IsNullOrEmpty(cachedResponse))
			{
				context.Result = new ContentResult
				{
					Content = cachedResponse,
					ContentType = "application/json",
					StatusCode = 200
				};
				return;
			}

			var executedContext = await next();

			if (executedContext.Result is OkObjectResult objectResult)
				await cacheService.CacheResponseAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
		}

		private static string GenerateCacheKeyFromRequest(HttpRequest request)
		{
			var keyBuilder = new StringBuilder();
			keyBuilder.Append($"{request.Path}");

			foreach (var (key, value) in request.Query.OrderBy(k => k.Key))
				keyBuilder.Append($"|{key}-{value}");

			return keyBuilder.ToString();
		}
	}
}
