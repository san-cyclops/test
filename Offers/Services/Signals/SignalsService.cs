using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Cache;
using Offers.Services.FileSave;
using Offers.Services.Locations;
using Offers.Services.Offers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public class SignalsService : ISignalsService<SignalsBaseEntity>
    {
       

        #region SignalsServiceFields
        private readonly ILogger<SignalsService> _Logger;
        private readonly ISignalBucketService<SignalsBaseEntity> _SignalBucketServiceCaller;
        private readonly IGetBaseOfferService<SignalsBaseEntity> _GetBaseOfferCaller;
        private readonly IGetOffersService<SignalsBaseEntity> _GetOffersServiceCaller;
        private readonly IGetAlternattiveOffers<SignalsBaseEntity> _GetAlternattiveOffersCaller;
        private readonly IFileSaveService<FileSaveBaseEntity> _IFileSave;
        private readonly ISignalsPaidService<SignalsBaseEntity> _SignalPaidCaller;
        #endregion

        #region SignalsServiceConstuctor
        public SignalsService(ISignalsPaidService<SignalsBaseEntity> SignalPaidCaller, IGetAlternattiveOffers<SignalsBaseEntity> GetAlternattiveOffersCaller, IGetOffersService<SignalsBaseEntity> GetOffersServiceCaller, IGetBaseOfferService<SignalsBaseEntity> GetBaseOfferCaller,ISignalBucketService<SignalsBaseEntity> SignalBucketServiceCaller,ILogger<SignalsService> Logger,IFileSaveService<FileSaveBaseEntity> IFileSave)
        {
            _SignalPaidCaller = SignalPaidCaller;
            _Logger = Logger;
            _SignalBucketServiceCaller = SignalBucketServiceCaller;
            _GetBaseOfferCaller = GetBaseOfferCaller;
            _GetOffersServiceCaller = GetOffersServiceCaller;
            _GetAlternattiveOffersCaller = GetAlternattiveOffersCaller;
            _IFileSave = IFileSave;

        }
        #endregion

        /*
           * Entry point after hit on the controller from front-end
           */
        #region GetSignals
        public async Task<QuadrantsRS> GetSignals(FlightSearchQuery_v2 Query)
        {
            try
            {
                QuadrantsRS QuadrantsRSObj = new QuadrantsRS();
                QuadrantsRSObj = await SignalsBaseCaller(Query);
                await  _IFileSave.Save_v1(Query,"Signal-RQ",Query.TransactionId);
                await  _IFileSave.Save_v1(Query, "SignalQuardant-RQ", Query.TransactionId);
                return await Task.FromResult(QuadrantsRSObj);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation(ErrorMessage.StackTrace);
                _Logger.LogError(ErrorMessage.StackTrace);
                return null;
            }

        }
        #endregion

        /*
           * Method which invoke offers and get Base offer from cacche for further calculation and send thoose offers to signal bucket .
           */

        #region SignalsBaseCaller
        public async Task<QuadrantsRS> SignalsBaseCaller(FlightSearchQuery_v2 Query)
        {
            try
            {
                QuadrantsRS QuadrantsRSObj = new QuadrantsRS();
                List<FlightResponse_v1> FlightResponse_v1ListAlternative = new List<FlightResponse_v1>();
                List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();

                //Get Base Offer
                BaseOfferRs BaseOfferRsObj = await _GetBaseOfferCaller.GetBaseOffer(Query.BaselineOffer);

                //if (BaselineOffer == null)
                //{
                //    ErrorRsObj = new ErrorRs();
                //    ErrorRsObj.detail = "Cache not contained related offer - CacheID -" + Query.BaselineOffer;
                //    ErrorRsObj.code = 206;
                //    ErrorRsLstObj.Add(ErrorRsObj);
                //    SignalsRsObj.ErrorList = new List<ErrorRs>();
                //    SignalsRsObj.ErrorList = ErrorRsLstObj;
                //}

                //Get related Offers
                FlightResponse_v1List = await _GetOffersServiceCaller.GetOffers(Query);

                //Get alternative Offers
                FlightResponse_v1ListAlternative = await _GetAlternattiveOffersCaller.GetOffers(Query.ArrivalLocation, Query);

                //Add Both Offers to list 
                FlightResponse_v1List.AddRange(FlightResponse_v1ListAlternative);

                //Invoke Signal Bucket to execute signals logic according to the quadrant
                
                QuadrantsRSObj = await _SignalPaidCaller.SignalsPaidServiceBaseCaller(FlightResponse_v1List, Query.Context, Query.TransactionId, BaseOfferRsObj.BaseOffer);
                    //current signals response
                QuadrantsRSObj.overview = await _SignalBucketServiceCaller.SignalBucketBaseCaller(FlightResponse_v1List, BaseOfferRsObj.BaseOffer, Query.TransactionId, Query.Context);

                QuadrantsRSObj.ErrorList = BaseOfferRsObj.ErrorList;

                return await Task.FromResult(QuadrantsRSObj);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation(ErrorMessage.StackTrace);
                _Logger.LogError(ErrorMessage.StackTrace);
                return null;
            }

        }
        #endregion
    }
}
