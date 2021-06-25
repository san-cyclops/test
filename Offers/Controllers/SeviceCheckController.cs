using Microsoft.AspNetCore.Mvc;

namespace Offers.Controllers
{
    [Route("offers/v1/[controller]")]
    [ApiController]
    public class SeviceCheckController : ControllerBase
    {
        #region PingApiCall
        [HttpGet]
        public string Ping()
        {
            return "Service Check Pinned at Offers";
        }
        #endregion

    }
}
