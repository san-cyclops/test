  using Microsoft.Extensions.Logging;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Locations;
using Offers.Services.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public class GetBaseOfferService : IGetBaseOfferService<SignalsBaseEntity>
    {
        #region SignalsServiceFields
        private readonly IOffersService<OffersBaseEntity> _OffersServiceCaller;
        private readonly ILocationsService<LocationsBaseEntity> _LocationsServiceCaller;
        private readonly ICacheService<CacheBaseEntity> _CacheCaller;
        private readonly ILogger<SignalsService> _Logger;
        private readonly IFileSaveService<FileSaveBaseEntity> _IFileSave;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;
        #endregion

        #region SignalsServiceConstuctor
        public GetBaseOfferService(IOffersService<OffersBaseEntity> OffersServiceCaller, ILocationsService<LocationsBaseEntity> LocationsServiceCaller, ICacheService<CacheBaseEntity> CacheCaller, ILogger<SignalsService> Logger, IFileSaveService<FileSaveBaseEntity> IFileSave, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _OffersServiceCaller = OffersServiceCaller;
            _LocationsServiceCaller = LocationsServiceCaller;
            _CacheCaller = CacheCaller;
            _Logger = Logger;
            _IFileSave = IFileSave;
            _ErrorLog = ErrorLogCaller;
        }
        #endregion

        /*
         * This service will create related Baseline offer from taking the offer from cache    
         */


        #region GetBaseOffer
        public async Task<BaseOfferRs> GetBaseOffer(String Id)
        {
            try
            {
                FlightResponse_v1 BaseOfferObj = new FlightResponse_v1();
                BaseOfferRs BaseOfferRsObj = new BaseOfferRs();
                BaseOfferObj = await _CacheCaller.GetOffer(Id);

                double seatpitchOutbound = BaseOfferObj.outbound.ratings.seatPrivacy;
                double seatprivacyOutbound = BaseOfferObj.outbound.ratings.seatPrivacy;
                double seatwidthOutbound = BaseOfferObj.outbound.ratings.seatWidth;
                double seattypeOutbound =  BaseOfferObj.outbound.ratings.seatType;
                double seatreclineOutbound = BaseOfferObj.outbound.ratings.seatRecline;


                String cabin = BaseOfferObj.outbound.@class.Substring(0,1)+ BaseOfferObj.inbound.@class.Substring(0, 1);

                if(String.IsNullOrEmpty(cabin))
                {
                    BaseOfferRsObj.ErrorList = new List<ErrorRs>();
                    BaseOfferRsObj.ErrorList.Add(await _ErrorLog.ErrorLog(Id , "Cabin Is Null or Empty"));
                }
                 
                double BaseOfferRatingOutbound = await RatingCalculationLogic(seatpitchOutbound, seatprivacyOutbound, seatreclineOutbound, seatreclineOutbound);

                double BaseOfferRating = BaseOfferRatingOutbound;

                BaseOfferRsObj.BaseOffer = BaseOfferObj.outbound.carrier.code + "," + BaseOfferObj.outbound.flight+ "," + "1" + "," + cabin + "," + BaseOfferRating + "," + BaseOfferObj.cost.amount + "," + BaseOfferObj.outbound.departure.at.Substring(0, 10);
                
                _Logger.LogInformation("BaseLine Offer >>>>>>>>> "+ BaseOfferRsObj.BaseOffer);

                return await Task.FromResult(BaseOfferRsObj);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation("Exception from GetbaseOfferService");
                _Logger.LogError(ErrorMessage.StackTrace);
                return null;
            }
        }
        #endregion

        #region RatingCalculationLogic
        public async Task<double> RatingCalculationLogic(double SeatPitch, double SeatPrivacy, double SeatRecline, double SeatWidth)
        {
             return await Task.FromResult((SeatPitch + SeatPrivacy + SeatRecline  + SeatWidth) / 4.0);
        }
        #endregion


    }
}
