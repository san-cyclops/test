using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Offer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Controllers
{
    [Route("offers/v1/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService<OfferBaseEntity> _SeatRatingRepositorycaller;
        private readonly ICacheService<CacheBaseEntity> _CacheRepositoryCaller;
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly ILogger<OfferController> _Logger;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;

        public OfferController(IOfferService<OfferBaseEntity> SeatRatingRepositorycaller, ICacheService<CacheBaseEntity> CacheRepositoryCaller, IFileSaveService<FileSaveBaseEntity> FileSaveCaller,ILogger<OfferController> Logger, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _SeatRatingRepositorycaller = SeatRatingRepositorycaller;
            _CacheRepositoryCaller = CacheRepositoryCaller;
            _FileSaveCaller = FileSaveCaller;
            _Logger = Logger;
            _ErrorLog = ErrorLogCaller;
        }



        //[Authorize]
        [HttpGet]
        public async Task<List<FlightResponse_v2>> GetSeatRatings([FromQuery] string[] uids,string TransactionId,string Context)
        {
            var OfferIdForSave = uids;
            await  _FileSaveCaller.Save_v1(OfferIdForSave, "Offer-RQ",TransactionId);
            try
            {
                return await _SeatRatingRepositorycaller.OfferBaseCaller(uids, TransactionId, Context);
            }
            catch (Exception ErrorMesage)
            {
                _Logger.LogError(ErrorMesage.StackTrace);
                return null;
            }
        }
    }
}
