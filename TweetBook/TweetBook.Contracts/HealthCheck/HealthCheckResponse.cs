using System;
using System.Collections.Generic;

namespace TweetBook.Contracts.HealthCheck
{
	public class HealthCheckResponse
	{
		public string Status { get; set; }
		public IEnumerable<HealthCheck> HealthChecks { get; set; }
		public TimeSpan Duration { get; set; }
	}
}
