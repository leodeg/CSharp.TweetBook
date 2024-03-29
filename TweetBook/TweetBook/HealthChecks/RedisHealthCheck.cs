﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TweetBook.HealthChecks
{
	public class RedisHealthCheck : IHealthCheck
	{
		private readonly IConnectionMultiplexer _connectionMultiplexer;

		public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
		{
			this._connectionMultiplexer = connectionMultiplexer;
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				var database = _connectionMultiplexer.GetDatabase();
				database.StringGet("health");
				return Task.FromResult(HealthCheckResult.Healthy());
			}
			catch (Exception ex)
			{
				return Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
			}
		}
	}
}
