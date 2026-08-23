using Microsoft.AspNetCore.Mvc;
using TrustedTools;
using TrustedAbstractions;

namespace TrustedExchangeWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatesController(
        IRateApi rateApi,
        ILogger<RatesController> logger) : ControllerBase
    {
        // GET /rates/USD
        [HttpGet("{currencyCode}")]
        [ProducesResponseType(typeof(RateRecord), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RateRecord), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RateRecord?>> GetUsdRate(string currencyCode)
        {
            try
            {
                var rate = await rateApi.GetUsdRate(currencyCode);
                return Ok(new RateRecord(rate, Normalise.CurrencyCode(currencyCode)));
            }
            catch (ArgumentException exception)
            {
                logger.LogWarning(exception,
                    "Unsupported currency code requested: {currencyCode}",
                    currencyCode);
                return NotFound();
            }
        }

        // Get /rates
        [HttpGet]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<string>>> GetSupportedCurrencyCodes()
        {
            var currencyCodes = await rateApi.GetSupportedCurrencyCodes();
            return Ok(currencyCodes);
        }
    }
}
