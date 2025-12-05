using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MyServiceCoin.Configuration;
using System.Net.NetworkInformation;

namespace MyServiceCoin.HealthChecks
{
    public class CoinsInfoHealthCheck : IHealthCheck
    {
        private readonly ServiceSetting _settings;

        public CoinsInfoHealthCheck(IOptions<ServiceSetting> settings)
        {
            _settings = settings.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            Ping ping = new();
            var url = "172.67.68.79";
            var reply = await ping.SendPingAsync(url);

            if (reply.Status != IPStatus.Success)
            {
                return HealthCheckResult.Unhealthy();
            }

            return HealthCheckResult.Healthy();
        }
    }
}
