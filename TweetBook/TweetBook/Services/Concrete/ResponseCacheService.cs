using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Services
{
	public class ResponseCacheService : IResponseCacheService
	{
		private readonly IDistributedCache _distributedCache;

		public ResponseCacheService(IDistributedCache distributedCache)
		{
			this._distributedCache = distributedCache;
		}

		public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
		{
			if (response == null)
				return;

			var serializedResponse = JsonConvert.SerializeObject(response);
			var distributedCacheEntryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeToLive };
			await _distributedCache.SetStringAsync(cacheKey, serializedResponse, distributedCacheEntryOptions);
		}

		public async Task<string> GetCachedResponseAsync(string cacheKey)
		{
			var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);
			return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
		}
	}
}
