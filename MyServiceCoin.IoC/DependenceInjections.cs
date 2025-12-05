using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace MyServiceCoin.IoC
{
    public static class DependenceInjections
    {
        public static DependenceInjections(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServiceSetting>(configuration.GetSection(nameof(ServiceSetting)));
            services.AddScoped<IApiClient, ApiClient>();
        }
    }
}
