using static MyServiceCoin.Services.ApiClient;

namespace MyServiceCoin.Services
{
    public interface IApiClient
    {
        CoinsInfo ConnectToApi(string currency);
    }
}
