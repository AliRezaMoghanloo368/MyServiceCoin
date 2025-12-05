using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MyServiceCoin.Configuration;
using Polly;
using RestSharp;
using System.Net;
using System.Runtime;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace MyServiceCoin.Services
{
    public class ApiClient : IApiClient
    {
        private readonly ServiceSetting _settings;
        private readonly ILogger<ApiClient> _logger;
        public ApiClient(IOptions<ServiceSetting> settings, ILogger<ApiClient> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        private static readonly List<HttpStatusCode> invalidStatusCode = new List<HttpStatusCode>
        {
            HttpStatusCode.BadRequest,
            HttpStatusCode.BadGateway,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.Forbidden,
            HttpStatusCode.GatewayTimeout
        };

        public CoinsInfo ConnectToApi(string currency)
        {
            //polly
            var retryPolicy = Policy
            .HandleResult<RestResponse>(resp => invalidStatusCode.Contains(resp.StatusCode))
            .WaitAndRetry(10, i => TimeSpan.FromSeconds(Math.Pow(2, i)), (result, TimeSpan, currentRetryCount, context) =>
            {
                _logger.LogError($"Request has failed with a {result.Result.StatusCode}. Waiting {TimeSpan} before next retry . This is the {currentRetryCount} retry");
            });

            var client = new RestClient($"{_settings.CoinsPriceUrl}/ticker");
            var request = new RestRequest("", Method.Get);
            request.RequestFormat = DataFormat.Json;

            request.AddParameter(name: "key", _settings.ApiKey, ParameterType.GetOrPost);
            request.AddParameter(name: "label", value: "ethbtc-ltcbtc-btcbtc", ParameterType.GetOrPost);
            request.AddParameter(name: "fiat", value: "usd", ParameterType.GetOrPost);

            var policyResponse = retryPolicy.ExecuteAndCapture(() =>
            {
                return client.Get(request);
            });

            if (policyResponse.Result != null)
            {
                return JsonSerializer.Deserialize<CoinsInfo>(policyResponse.Result.Content);
            }
            else
            {
                return null; 
            }
        }

        public record Market(string Label, string Name, double Price);
        public record CoinsInfo(Market[] Markets);
    }
}
