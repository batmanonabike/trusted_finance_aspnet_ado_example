using Microsoft.AspNetCore.Mvc;
using TrustedTools;
using TrustedAbstractions;

namespace TrustedExchangeWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RateController : ControllerBase
    {
        // GET /api/rate/USD
        [HttpGet("{currencyCode}")]
        [ProducesResponseType(typeof(RateRecord), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RateRecord), StatusCodes.Status404NotFound)]
        public ActionResult<RateRecord?> GetUsdRate(string currencyCode)
        {
            return Ok(new RateRecord(12.33m, Normalise.CurrencyCode(currencyCode)));
        }
    }
}
