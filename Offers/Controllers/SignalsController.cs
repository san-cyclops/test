using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Controllers
{
    [Route("offers/v1/[controller]")]
    [ApiController]
    public class SignalsController : ControllerBase
    {
        private readonly ISignalsService<SignalsBaseEntity> _SignalsServicecaller;

        public SignalsController(ISignalsService<SignalsBaseEntity> SignalsServicecaller)
        {
            _SignalsServicecaller = SignalsServicecaller ;
        }


       //[Authorize]
        [HttpGet]
        public async Task<QuadrantsRS> GetSeatRatings([FromQuery] FlightSearchQuery_v2 RequestQuery)
        {
            try
            {
                return await _SignalsServicecaller.GetSignals(RequestQuery);
            }
            catch (Exception ErrorMesage)
            {
                Console.WriteLine(ErrorMesage);
                return null;
            }
        }
    }
}
