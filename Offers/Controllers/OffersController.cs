using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.FileSave;
using Offers.Services.Offers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Controllers
{
    [Route("offers/v1/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {

        #region Fields
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly ILogger<OffersController> _logger;
        private readonly IOffersService<OffersBaseEntity> _Amadeus_SearchRepositoryCaller;
        #endregion

        #region OffersConstroctor
        public OffersController(ILogger<OffersController> logger, IOffersService<OffersBaseEntity> Amadeus_SearchRepositoryCaller,IFileSaveService<FileSaveBaseEntity>FileSaveCaller)
        {
            _FileSaveCaller = FileSaveCaller;
            _logger = logger;
            _Amadeus_SearchRepositoryCaller = Amadeus_SearchRepositoryCaller;
        }
        #endregion

        #region OffersApiCall
       // [Authorize]
        [HttpGet]
        public async Task<List<FlightResponse_v1>> Search([FromQuery] FlightSearchQuery_v1 RequestQuery)
        {
            try
            {
                // _logger.LogDebug("response"+ response);
                _logger.LogInformation("Transaction Id :" +RequestQuery.TransactionId);
                return (await _Amadeus_SearchRepositoryCaller.FlightOffersSearch(RequestQuery));

            }
            catch (Exception ErrorMesage)
            {
                /* Should have provide meaning full error that recvieved from supplier example : no results for that date
                 * Need create errorObj and added to the FlightOffersSearchRS
                 */
                await _FileSaveCaller.Save_v1(ErrorMesage,"Error-Offers", RequestQuery.TransactionId);
                _logger.LogInformation("Transaction Id :" + RequestQuery.TransactionId +"is NUll because of exception");
                _logger.LogInformation(ErrorMesage.ToString());
                _logger.LogError(ErrorMesage.StackTrace);
                return null;
            }
        }
        #endregion

    }
}
