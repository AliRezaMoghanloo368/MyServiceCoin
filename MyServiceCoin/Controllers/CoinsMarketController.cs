using Microsoft.AspNetCore.Mvc;
using MyServiceCoin.Attributes;
using MyServiceCoin.Services;

namespace MyServiceCoin.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    [RequireHealthy("Coins API is down, please retry later")]
    public class CoinsMarketController : ControllerBase
    {
        private readonly IApiClient _apiClient;
        public CoinsMarketController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("{currency}")]
        public IActionResult GetAction(string currency)
        {
            var result = _apiClient.ConnectToApi(currency);
            return Ok(result);
        }
    }
}
