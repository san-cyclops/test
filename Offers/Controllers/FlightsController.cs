using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.FileSave;
using Offers.Services.Flights;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flights.Controllers
{
    [Route("flights")]
    [ApiController]
    public class FlightsController : ControllerBase
    {

        #region Fields
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly ILogger<FlightsController> _logger;
        private readonly IFlightsService<OffersBaseEntity> _Amadeus_SearchRepositoryCaller;
        #endregion

        #region FlightsConstroctor
        public FlightsController(ILogger<FlightsController> logger, IFlightsService<OffersBaseEntity> Amadeus_SearchRepositoryCaller,IFileSaveService<FileSaveBaseEntity>FileSaveCaller)
        {
            _FileSaveCaller = FileSaveCaller;
            _logger = logger;
            _Amadeus_SearchRepositoryCaller = Amadeus_SearchRepositoryCaller;
        }
        #endregion

        #region FlightsApiCall
        // [Authorize]
        [HttpGet]
        public async Task<object> FlightsSearch([FromQuery] FlightSearchQuery_v3 RequestQuery)
        {
            try
            {
                _logger.LogInformation("Transaction Id :" +RequestQuery.TransactionId);
                List<FlightResponse_v3> ResponeList = await _Amadeus_SearchRepositoryCaller.FlightsSearchBaseCaller(RequestQuery);
                return Ok(ResponeList);
            }
            catch (Exception ErrorMesage)
            {
                await _FileSaveCaller.Save_v1(ErrorMesage,"Error-Flights", RequestQuery.TransactionId);
                _logger.LogInformation("Transaction Id :" + RequestQuery.TransactionId +"is NUll because of exception");
                _logger.LogInformation(ErrorMesage.ToString());
                _logger.LogError(ErrorMesage.StackTrace);
                return null;
            }
        }
        #endregion

    }
}
