using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Alliances;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Locations;
using Offers.Services.Offers;
using Offers.Services.UserRating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public class SignalBucketService : ISignalBucketService<SignalsBaseEntity>
    {
        #region SignalsServiceFields
        private readonly ILogger<SignalsService> _Logger;
        private readonly IFileSaveService<FileSaveBaseEntity> _IFileSave;
        private readonly IGetAlliances _IGetAlliances;
        private readonly IUserRatingService<UserRatingBaseEntity> _UserRating;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;
        #endregion

        #region SignalBucketConstructor
        public SignalBucketService(ILogger<SignalsService> Logger, IFileSaveService<FileSaveBaseEntity> IFileSave, IGetAlliances IGetAlliances,IUserRatingService<UserRatingBaseEntity> UserRating, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _Logger = Logger;
            _IFileSave = IFileSave;
            _IGetAlliances = IGetAlliances;
            _UserRating = UserRating;
            _ErrorLog = ErrorLogCaller;
        }
        #endregion

          /*
           * The entry point of signals for execute diffecrent kind of services to get signals using offers list and alternattive offers list .
           */

        #region SignalBucketServiceBase
        public async Task<SignalsRs_v1> SignalBucketBaseCaller(List<FlightResponse_v1> FlightResponse_v1List,String BaselineOffer,String TransactionID,string Context)
        {
          List<SignalsBucketRs_v1> SignalsBucketList = new List<SignalsBucketRs_v1>();
            SignalsRs_v1 SignalsRsObj = new SignalsRs_v1();

            SignalsBucketList = await SignalBucket(FlightResponse_v1List,BaselineOffer,TransactionID,Context);
            SignalsRsObj = await ResponseBuilder(SignalsBucketList, TransactionID,BaselineOffer);

            return await Task.FromResult(SignalsRsObj);
        }
        #endregion

        /*
           * Signal bucket is the placewhich we digging list of signals from offers 
           */
        #region SignalBucket
        public async Task<List<SignalsBucketRs_v1>> SignalBucket(List<FlightResponse_v1> FlightResponse_v1List,String BaseLineFlight,String TransactionID,String Context)
        {

            try
            {
                //TEST SIGNALS
                // BaseLineFlight = "BA,1,1,P,3.5,2100,2021-04-04";

                List<SignalsBucketRs_v1> SignalsBucketList = new List<SignalsBucketRs_v1>();
                string[] DataList = BaseLineFlight.Split(",");
                string BaseFlightDate = DataList[6];
                if (FlightResponse_v1List.Count > 0)
                {
                    //TEST SIGNALS
                    // FlightResponse_v1List = null;
                    // FlightResponse_v1List = JsonConvert.DeserializeObject<List<FlightResponse_v1>>(File.ReadAllText(@"D:\Ozen Labs\FCF\SIGNALS_qa\SPECIFICATIONS.json"));
                    foreach (var item in FlightResponse_v1List)
                    {
                        Boolean AddedToBucket = false;
                        SignalsBucketRs_v1 SignalsBucketRs = new SignalsBucketRs_v1();
                        SignalsBucketRs.Date = ((int)DateTime.Parse(item.outbound.departure.at.Substring(0,10)).Subtract(DateTime.Parse(BaseFlightDate)).TotalDays);
                        SignalsBucketRs.Destination = item.outbound.arrival.code;
                        SignalsBucketRs.Airline = item.outbound.carrier.code;
                        SignalsBucketRs.Flight = item.outbound.flight;
                        SignalsBucketRs.Cabin = item.outbound.@class.Substring(0,1) + item.inbound.@class.Substring(0, 1);
                        SignalsBucketRs.Price = item.cost.amount;
                        SignalsBucketRs.Alliances = item.outbound.carrier.code;
                        SignalsBucketRs.OfferID = item.id;
                        /*
                         * With FCF Rating
                         */
                        //double OutboundRating = await RatingCalculationLogic(item.outbound.ratings.seatPitch, item.outbound.ratings.seatPrivacy, item.outbound.ratings.seatRecline, item.outbound.ratings.seatType, item.outbound.ratings.seatWidth);
                        //SignalsBucketRs.Rating = OutboundRating;

                        /*
                         * With UserWeightnings
                         */
                        UserRatingRs UserRatingRsObj = new UserRatingRs();
                        UserRatingRsObj = await _UserRating.UserRating(item.outbound.ratings,Context,TransactionID);
                        if (UserRatingRsObj.Value == 0)
                        {
                            _Logger.LogError("Rating null when User Rating executed at SinglebucketService : Check Error folder for related Logs" +TransactionID);
                            await _IFileSave.Save_v1(item.outbound.ratings,"Error", TransactionID);
                            await _IFileSave.Save_v1(Context, "Error", TransactionID);

                            SignalsBucketRs.ErrorList = new List<ErrorRs>();
                            SignalsBucketRs.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionID, "Rating null when User Rating executed at SinglebucketService : Check Error folder for related Logs"));
                        }
                        //if (UserRatingRsObj.ErrorList.Count != 0)
                        //{
                        //    _Logger.LogError("Rating null when User Rating executed at SinglebucketService : Check Error folder for related Logs" +TransactionID);
                        //    await _IFileSave.Save_v1(item.outbound.ratings,"Error", TransactionID);
                        //    await _IFileSave.Save_v1(Context, "Error", TransactionID);
                        //    ErrorRsObj = new ErrorRs();
                        //    ErrorRsObj.detail = "Rating null when User Rating executed at SinglebucketService : Check Error folder for related Logs" + TransactionID;
                        //    ErrorRsLstObj.Add(ErrorRsObj);
                        //    SignalsBucketRs.ErrorList = new List<ErrorRs>();
                        //    SignalsBucketRs.ErrorList = ErrorRsLstObj;
                        //}
                        else
                        { 
                            SignalsBucketRs.Rating = UserRatingRsObj.Value;

                        SignalsBucketRs = await SignalDeltaCalculationLogic(SignalsBucketRs, BaseLineFlight);
                        if (SignalsBucketRs.DeltaValue > 0.00)
                        {
                            String Key = SignalsBucketRs.Airline + "," + SignalsBucketRs.Flight + ","+"1," + SignalsBucketRs.Cabin + "," + SignalsBucketRs.Rating + "," + SignalsBucketRs.Price + "," + item.outbound.departure.at.Substring(0, 10);
                            if (Key != BaseLineFlight)
                            {
                                    
                                    _Logger.LogInformation("Key >>>>> " + Key);
                                    AddedToBucket = true;
                                    SignalsBucketList.Add(SignalsBucketRs);
                                
                                }
                            }
                            await  _IFileSave.Save_v1(SignalsBucketRs,"SignalBucketResponse_"+TransactionID, SignalsBucketRs.Date +"_"+ SignalsBucketRs.Destination +"_"+ SignalsBucketRs.Airline +"_"+ SignalsBucketRs.Cabin+"_"+AddedToBucket+"_");
                        }
                    }
                }

                try
                {
                    string[] Airlines = SignalsBucketList.Select(p => p.Airline).Distinct().ToArray();
                    List<AirlineRS> AirlineRSList = await _IGetAlliances.GetAlliances(Airlines);

                    foreach (SignalsBucketRs_v1 item in SignalsBucketList)
                    {
                        bool FindAlliances = false;
                        foreach (var AirlineList in AirlineRSList)
                        {
                            foreach (var Alliances in AirlineList.airlines)
                            {
                                if (item.Alliances == Alliances.iata)
                                {
                                    FindAlliances = true;
                                    item.Alliances = AirlineList.name;
                                }
                            }
                               
                        }
                        if (!FindAlliances)
                        {
                            item.Alliances = "Non Alliance";
                        }

                    }
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogInformation("Exception in Signal Bucket Get Alliance");
                    _Logger.LogError(ErrorMessage.StackTrace);
                    return null;
                }

                return await Task.FromResult(SignalsBucketList);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation("Exception in Signal Bucket");
                _Logger.LogError(ErrorMessage.StackTrace);
                return null;
            }
        }
        #endregion

        #region RatingCalculationLogic
            public async Task<double> RatingCalculationLogic(double SeatPitch, double SeatPrivacy, double SeatRecline, double SeatType, double SeatWidth)
        {
             return await Task.FromResult((SeatPitch + SeatPrivacy + SeatRecline + SeatType + SeatWidth) / 4.0);
        }
        #endregion

      
        /*
           * This Moethod will calulate Logic which we need to get signals further delta calculations of FCF
           */

        #region SignalDeltaCalculationLogic
        public async Task<SignalsBucketRs_v1> SignalDeltaCalculationLogic(SignalsBucketRs_v1 SignalsBucketRs, String BaseLineFlight)
        {
            try
            {
                string[] DataList = BaseLineFlight.Split(",");
                double BaseFlightRating = Convert.ToDouble(DataList[4]);
                double BaseFlightPrice = Convert.ToDouble(DataList[5]);

                double BaseFlightvalueRatio = Math.Round(((BaseFlightRating / BaseFlightPrice) * 1000),7);

                SignalsBucketRs.ValueRatio = Math.Round(((SignalsBucketRs.Rating / SignalsBucketRs.Price) * 1000), 2);
                SignalsBucketRs.DeltaRating = Math.Round((SignalsBucketRs.Rating - BaseFlightRating), 1);
                SignalsBucketRs.DeltaPrice = SignalsBucketRs.Price - BaseFlightPrice;
                double DeltaValue = Math.Round((SignalsBucketRs.ValueRatio - BaseFlightvalueRatio), 2);
                if (DeltaValue > 0)
                {
                    SignalsBucketRs.DeltaValue = DeltaValue;
                }

                return await Task.FromResult(SignalsBucketRs);
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
           * This method query and Format Signals which we have at signal bucket for send to the  FCF Format for FrontEnd 
           */
        #region ResponseBuilder
        public async Task<SignalsRs_v1> ResponseBuilder(List<SignalsBucketRs_v1> SignalsBucketRsList, String TransactionID, String BaseLineFlight)
        {
            await _IFileSave.Save_v1(SignalsBucketRsList, "SignalsBucketRs_",TransactionID);
            try
            {
                string[] DataList = BaseLineFlight.Split(",");
                double BaseFlightRating = Convert.ToDouble(DataList[4]);
                double BaseFlightPrice = Convert.ToDouble(DataList[5]);

                SignalsRs_v1 SignalsRs = new SignalsRs_v1();
                SignalsRs.BaselineRating = BaseFlightRating;
                SignalsRs.BaselinePrice = BaseFlightPrice;

                List<Destination> DestinationsList = new List<Destination>();
                List<Alliance> AlliancesList = new List<Alliance>();
                List<Airline> AirlinesList = new List<Airline>();
                List<DateFlexibility> DateFlexibilityList = new List<DateFlexibility>();
                MixedCabin MixedCabinObj = new MixedCabin();
                List<Cabin> cabinsList = new List<Cabin>();

                var MixedCabinDeltaPriceMin = SignalsBucketRsList.Select(p => p.DeltaPrice).Min();
                var MixedCabinDeltaPriceMax = SignalsBucketRsList.Select(p => p.DeltaPrice).Max();
                var MixedCabinDeltaRatingMin = SignalsBucketRsList.Select(p => p.DeltaRating).Min();
                var MixedCabinDeltaRatingMax = SignalsBucketRsList.Select(p => p.DeltaRating).Max();
                var MixedCabinDeltaValueMin = SignalsBucketRsList.Select(p => p.DeltaValue).Min();
                var MixedCabinDeltaValueMax = SignalsBucketRsList.Select(p => p.DeltaValue).Max();

                MixedCabinObj.maxPriceDiff = MixedCabinDeltaPriceMax;
                MixedCabinObj.minPriceDiff = MixedCabinDeltaPriceMin;
                MixedCabinObj.minRatingDiff = MixedCabinDeltaRatingMin;
                MixedCabinObj.maxRatingDiff = MixedCabinDeltaRatingMax;
                MixedCabinObj.minValueDiff = MixedCabinDeltaValueMin;
                MixedCabinObj.maxValueDiff = MixedCabinDeltaValueMax;
                SignalsRs.mixedCabin = MixedCabinObj;

                var Destination = SignalsBucketRsList.Select(data => new { data.Destination }).Distinct().ToList();
                foreach (var item in Destination)
                {

                    var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Destination == item.Destination).Select(p => p.DeltaPrice).Min();
                    var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Destination == item.Destination).Select(p => p.DeltaPrice).Max();
                    var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Destination == item.Destination).Select(p => p.DeltaRating).Min();
                    var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Destination == item.Destination).Select(p => p.DeltaRating).Max();
                    var DeltaValueMin = SignalsBucketRsList.Where(m => m.Destination == item.Destination).Select(p => p.DeltaValue).Min();
                    var DeltaValueMax = SignalsBucketRsList.Where(m => m.Destination == item.Destination).Select(p => p.DeltaValue).Max();

                    Destination DestinationObj = new Destination();
                    DestinationObj.code = item.Destination;
                    DestinationObj.maxPriceDiff = DeltaPriceMax;
                    DestinationObj.minPriceDiff = DeltaPriceMin;
                    DestinationObj.minRatingDiff = DeltaRatingMin;
                    DestinationObj.maxRatingDiff = DeltaRatingMax;
                    DestinationObj.minValueDiff = DeltaValueMin;
                    DestinationObj.maxValueDiff = DeltaValueMax;

                    DestinationsList.Add(DestinationObj);

                }

                var Airlines = SignalsBucketRsList.Select(data => new { data.Airline }).Distinct().ToList();
                foreach (var item in Airlines)
                {
                    var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Airline == item.Airline).Select(p => p.DeltaPrice).Min();
                    var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Airline == item.Airline).Select(p => p.DeltaPrice).Max();
                    var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Airline == item.Airline).Select(p => p.DeltaRating).Min();
                    var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Airline == item.Airline).Select(p => p.DeltaRating).Max();
                    var DeltaValueMin = SignalsBucketRsList.Where(m => m.Airline == item.Airline).Select(p => p.DeltaValue).Min();
                    var DeltaValueMax = SignalsBucketRsList.Where(m => m.Airline == item.Airline).Select(p => p.DeltaValue).Max();

                    Airline AirlineObj = new Airline();
                    AirlineObj.code = item.Airline;
                    AirlineObj.maxPriceDiff = DeltaPriceMax;
                    AirlineObj.minPriceDiff = DeltaPriceMin;
                    AirlineObj.minRatingDiff = DeltaRatingMin;
                    AirlineObj.maxRatingDiff = DeltaRatingMax;
                    AirlineObj.minValueDiff = DeltaValueMin;
                    AirlineObj.maxValueDiff = DeltaValueMax;

                    AirlinesList.Add(AirlineObj);

                }

                var Alliances = SignalsBucketRsList.Select(data => new { data.Alliances }).Distinct().ToList();
                foreach (var item in Alliances)
                {
                    var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.DeltaPrice).Min();
                    var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.DeltaPrice).Max();
                    var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.DeltaRating).Min();
                    var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.DeltaRating).Max();
                    var DeltaValueMin = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.DeltaValue).Min();
                    var DeltaValueMax = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.DeltaValue).Max();

                    Alliance AllianceObj = new Alliance();
                    AllianceObj.code = item.Alliances;
                    AllianceObj.maxPriceDiff = DeltaPriceMax;
                    AllianceObj.minPriceDiff = DeltaPriceMin;
                    AllianceObj.minRatingDiff = DeltaRatingMin;
                    AllianceObj.maxRatingDiff = DeltaRatingMax;
                    AllianceObj.minValueDiff = DeltaValueMin;
                    AllianceObj.maxValueDiff = DeltaValueMax;

                    AlliancesList.Add(AllianceObj);

                }

                var DateFlexibility = SignalsBucketRsList.Select(data => new { data.Date }).Distinct().ToList();
                foreach (var item in DateFlexibility)
                {
                    var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.DeltaPrice).Min();
                    var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.DeltaPrice).Max();
                    var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.DeltaRating).Min();
                    var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.DeltaRating).Max();
                    var DeltaValueMin = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.DeltaValue).Min();
                    var DeltaValueMax = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.DeltaValue).Max();

                    DateFlexibility DateFlexibilityObj = new DateFlexibility();
                    DateFlexibilityObj.code = item.Date;
                    DateFlexibilityObj.maxPriceDiff = DeltaPriceMax;
                    DateFlexibilityObj.minPriceDiff = DeltaPriceMin;
                    DateFlexibilityObj.minRatingDiff = DeltaRatingMin;
                    DateFlexibilityObj.maxRatingDiff = DeltaRatingMax;
                    DateFlexibilityObj.minValueDiff = DeltaValueMin;
                    DateFlexibilityObj.maxValueDiff = DeltaValueMax;

                    DateFlexibilityList.Add(DateFlexibilityObj);

                }

                var Cabins = SignalsBucketRsList.Select(data => new { data.Cabin }).Distinct().ToList();

                foreach (var item in Cabins)
                {
                    var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Cabin == item.Cabin).Select(p => p.DeltaPrice).Min();
                    var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Cabin == item.Cabin).Select(p => p.DeltaPrice).Max();
                    var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Cabin == item.Cabin).Select(p => p.DeltaRating).Min();
                    var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Cabin == item.Cabin).Select(p => p.DeltaRating).Max();
                    var DeltaValueMin = SignalsBucketRsList.Where(m => m.Cabin == item.Cabin).Select(p => p.DeltaValue).Min();
                    var DeltaValueMax = SignalsBucketRsList.Where(m => m.Cabin == item.Cabin).Select(p => p.DeltaValue).Max();

                    Cabin CabinObj = new Cabin();
                    CabinObj.code = item.Cabin;
                    CabinObj.maxPriceDiff = DeltaPriceMax;
                    CabinObj.minPriceDiff = DeltaPriceMin;
                    CabinObj.minRatingDiff = DeltaRatingMin;
                    CabinObj.maxRatingDiff = DeltaRatingMax;
                    CabinObj.minValueDiff = DeltaValueMin;
                    CabinObj.maxValueDiff = DeltaValueMax;

                    cabinsList.Add(CabinObj);

                }

                SignalsRs.offerIds = SignalsBucketRsList.Select(O => O.OfferID).ToArray();
                SignalsRs.destinations = DestinationsList;
                SignalsRs.airlines = AirlinesList;
                SignalsRs.alliances = AlliancesList;
                SignalsRs.cabins = cabinsList;
                DateFlexibilityList = DateFlexibilityList.Where(p => p.code >= -3 && p.code <= 3).ToList();
                SignalsRs.dateFlexibility = DateFlexibilityList.OrderBy(d => d.code).ToList();

                return await Task.FromResult(SignalsRs);
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
