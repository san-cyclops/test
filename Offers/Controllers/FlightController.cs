using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Flight;
using Offers.Services.Offer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Controllers
{
    [Route("flight")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService<OfferBaseEntity> _Flightcaller;
        private readonly ICacheService<CacheBaseEntity> _CacheRepositoryCaller;
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly ILogger<OfferController> _Logger;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;
        public FlightController(IFlightService<OfferBaseEntity> Flightcaller, ICacheService<CacheBaseEntity> CacheRepositoryCaller, IFileSaveService<FileSaveBaseEntity> FileSaveCaller,ILogger<OfferController> Logger, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _Flightcaller = Flightcaller;
            _CacheRepositoryCaller = CacheRepositoryCaller;
            _FileSaveCaller = FileSaveCaller;
            _Logger = Logger;
            _ErrorLog = ErrorLogCaller;
        }



        //[Authorize]
        [HttpGet]
        public async Task<FlightResponse_v4> GetFlightRatings([FromQuery] string[] uids,string TransactionId,string Context)
        {
            var OfferIdForSave = uids;
            await  _FileSaveCaller.Save_v1(OfferIdForSave, "Flightr-RQ",TransactionId);
            try
            {
                return await _Flightcaller.FlightBaseCaller(uids,TransactionId,Context);
            }
            catch (Exception ErrorMesage)
            {
                _Logger.LogError(ErrorMesage.StackTrace);
                return null;
            }
        }
    }
}
