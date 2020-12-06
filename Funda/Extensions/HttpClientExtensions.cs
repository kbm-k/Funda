using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Funda.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetAsyncWithPolicy(this HttpClient httpClient, string url,
            AsyncRetryPolicy<HttpResponseMessage> retryPolicy = null,
            AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreakerPolicy = null)
        {
            var jitterer = new Random();

            retryPolicy ??= HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                .WaitAndRetryAsync(6,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );

            circuitBreakerPolicy ??= HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

            var policyWrap = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
            var policyResult = await policyWrap.ExecuteAndCaptureAsync(async () => await httpClient.GetAsync(url));

            return policyResult.Result;
        }
    }
}
