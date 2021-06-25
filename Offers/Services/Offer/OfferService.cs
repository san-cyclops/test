using Microsoft.Extensions.Logging;
using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Offer
{
    public class OfferService : IOfferService<OfferBaseEntity>
    {

        /*
* Logic for Ratings calculation Version 1 For SeatRating
* Metod LogicForRatingCalculation : Main Logic for rate Calculation FCF /Offer
* Metod SeatRatingFromCSV : Get Formula and ratings for seats using CSV
* Method RatingPickingLogic : Logic for applying rate Formula
*/
        private readonly ILogger<OfferService> _Logger;
        private readonly ISeatService<SeatBaseEntity> _SeatdataCaller;
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;
        private readonly ICacheService<CacheBaseEntity> _CacheRepositoryCaller;

        public OfferService(ISeatService<SeatBaseEntity> SeatdataCaller,ILogger<OfferService> Logger, ICacheService<CacheBaseEntity> CacheRepositoryCaller,IFileSaveService<FileSaveBaseEntity> FileSaveCaller, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _Logger = Logger;
            _SeatdataCaller = SeatdataCaller;
            _FileSaveCaller = FileSaveCaller;
            _ErrorLog = ErrorLogCaller;
            _CacheRepositoryCaller = CacheRepositoryCaller;
        }

 
        #region OfferBaseCaller
        public async Task<List<FlightResponse_v2>>OfferBaseCaller(string[] uids, string TransactionId, string Context)
        {
            var OfferIdForSave = uids;
            List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();
            List<SeatsRs> SeatDataRSList = new List<SeatsRs>();
            foreach (string item in uids)
            {
                _Logger.LogInformation("Offer-Id >>>>>>>>>> " + item);
                try
                {
                    FlightResponse_v1 FlightResponse_v1Obj = await _CacheRepositoryCaller.GetOffer(item);
                    FlightResponse_v1List.Add(FlightResponse_v1Obj);
                    await _FileSaveCaller.Save_v1("Uid-"+ uids + "@@" + "TransactionId-" +  TransactionId + "@@" + "Context-" + Context, "Cache-Offer-RQ", TransactionId);
                    await _FileSaveCaller.Save_v1(FlightResponse_v1List, "Cache-Offer-RS", TransactionId);
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogWarning("Offer-Id Not in Cache >>>>>>>>>> " + item);
                    _Logger.LogError("Offer-Id Not in cache  >>>>>>>>>> " + ErrorMessage.StackTrace);

                    FlightResponse_v1 FlightResponse_v1Obj = new FlightResponse_v1();
                    FlightResponse_v1Obj.ErrorList = new List<ErrorRs>();
                    FlightResponse_v1Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, "Cache Expired - Offer-Id Not in Cache"));
                    FlightResponse_v1List.Add(FlightResponse_v1Obj);
                }

            }
            SeatDataRSList = await SeatDataRequest(FlightResponse_v1List, TransactionId, Context);
            return await RatingCalculation(FlightResponse_v1List, TransactionId, Context, SeatDataRSList);
        }
        #endregion

        #region LogicForRatingCalculation
        public async Task<List<FlightResponse_v2>> RatingCalculation(List<FlightResponse_v1> FlightResponse_v1Obj,String TransactionId, string Context, List<SeatsRs> SeatDataRSList)
        {
            try
            {
                List<FlightResponse_v2> FlightResponse_v2List = new List<FlightResponse_v2>();
                foreach (var item in FlightResponse_v1Obj)
                {
                    String KeyOutbound = item.outbound.operating.code + "-" + item.outbound.aircraft.code + "-" + "1" + "-" + CabinClassFinder(item.outbound.@class);
                    String KeyInbound = item.inbound.operating.code + "-" + item.inbound.aircraft.code + "-" + "1" + "-" + CabinClassFinder(item.inbound.@class);

                    SeatsRs SeatDataRSInbound = new SeatsRs();
                    SeatsRs SeatDataRSOutbound = new SeatsRs();

                    //try
                    //{
                    //    List<string> Key = new List<string>();
                    //    Key.Add(KeyInbound);
                    SeatDataRSInbound = SeatDataRSList.Find(RatingInbound => RatingInbound.id == KeyInbound);
                    //    Key.Clear();
                    //    Key.Add(KeyOutbound);
                    SeatDataRSOutbound = SeatDataRSList.Find(RatingOutbound => RatingOutbound.id == KeyOutbound);
                    //}
                    //catch (Exception ErrorMessage)
                    //{
                    //    SeatDataRSInbound = null;
                    //    SeatDataRSOutbound = null;
                    //    _Logger.LogInformation(ErrorMessage.StackTrace);
                    //    _Logger.LogError(ErrorMessage.StackTrace);

                    //}

                    FlightResponse_v2 FlightResponse_v2Obj = new FlightResponse_v2();
                    FlightResponse_v2Obj.id = item.id;
                    Random RandomValue = new Random();
                    /*
                    * Logic for Ratings Departure Flight
                    * 
                    */

                    #region Departure Flight
                    DepartureFlight_v2 DepartureFlight_v2Obj = new DepartureFlight_v2();
                    DepartureFlight_v2Obj.duration = item.outbound.duration;

                    Departure_v2 Departure_v2Obj = new Departure_v2();
                    Departure_v2Obj.code = item.outbound.departure.code;
                    Departure_v2Obj.name = "";
                    Departure_v2Obj.city = "";
                    Departure_v2Obj.country = "";
                    Departure_v2Obj.at = item.outbound.departure.at;
                    DepartureFlight_v2Obj.departure = Departure_v2Obj;

                    Operating_v2 Operating_v2Obj = new Operating_v2();
                    Operating_v2Obj.code = item.outbound.operating.code;
                    Operating_v2Obj.name = item.outbound.operating.name;
                    DepartureFlight_v2Obj.operating = Operating_v2Obj;

                    Arrival_v2 Arrival_v2Obj = new Arrival_v2();
                    Arrival_v2Obj.code = item.outbound.arrival.code;
                    Arrival_v2Obj.name = "";
                    Arrival_v2Obj.city = "";
                    Arrival_v2Obj.country = "";
                    Arrival_v2Obj.at = item.outbound.arrival.at;
                    DepartureFlight_v2Obj.arrival = Arrival_v2Obj;

                    Carrier_v2 Carrier_v2Obj = new Carrier_v2();
                    Carrier_v2Obj.code = item.outbound.carrier.code;
                    Carrier_v2Obj.name = item.outbound.carrier.name;
                    DepartureFlight_v2Obj.carrier = Carrier_v2Obj;

                    Aircraft_v2 Aircraft_v2Obj = new Aircraft_v2();
                    Aircraft_v2Obj.code = item.outbound.aircraft.code;
                    DepartureFlight_v2Obj.aircraft = Aircraft_v2Obj;

                    Cabins_v2 CabinObj = new Cabins_v2();
                    CabinObj.cabinClass = item.outbound.@class;
                    List<Amenity_v2> Amenity_v2List = new List<Amenity_v2>();

                    Amenity_v2 Amenity_v2Obj = new Amenity_v2();
                    Amenity_v2Obj.name = "Seat Width";
                    Amenity_v2Obj.unit = "inches";
                    Amenity_v2Obj.key = "seatWidth";
                    if (SeatDataRSOutbound != null)
                    {
                        CabinObj.random = false;
                        Amenity_v2Obj.value = SeatDataRSOutbound.ratings.perAttribute.seatWidth.value;
                        Amenity_v2Obj.rating = Convert.ToDouble(SeatDataRSOutbound.ratings.perAttribute.seatWidth.rating);
                    }
                    else
                    {
                        CabinObj.random = true;
                        Amenity_v2Obj.value = RandomValue.Next(1,5).ToString();
                        Amenity_v2Obj.rating = Convert.ToDouble(RandomValue.Next(1, 5));

                        FlightResponse_v2Obj.ErrorList = new List<ErrorRs>();
                        FlightResponse_v2Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound random number generated"));
                    }

                    Amenity_v2List.Add(Amenity_v2Obj);

                    Amenity_v2 Amenity_v2ObjTwo = new Amenity_v2();
                    Amenity_v2ObjTwo.name = "Seat Pitch";
                    Amenity_v2ObjTwo.unit = "inches";
                    Amenity_v2ObjTwo.key = "seatPitch";
                    if (SeatDataRSOutbound != null)
                    {
                        Amenity_v2ObjTwo.value = SeatDataRSOutbound.ratings.perAttribute.seatPitch.value;
                        Amenity_v2ObjTwo.rating = Convert.ToDouble(SeatDataRSOutbound.ratings.perAttribute.seatPitch.rating);
                    }
                    else
                    {
                        Amenity_v2ObjTwo.value = RandomValue.Next(1, 5).ToString();
                        Amenity_v2ObjTwo.rating = Convert.ToDouble(RandomValue.Next(1, 5));

                        FlightResponse_v2Obj.ErrorList = new List<ErrorRs>();
                        FlightResponse_v2Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound random number generated"));
                    }
                    Amenity_v2List.Add(Amenity_v2ObjTwo);

                    Amenity_v2 Amenity_v2ObjFive = new Amenity_v2();
                    Amenity_v2ObjFive.name = "Seat Recline";
                    Amenity_v2ObjFive.unit = " ";
                    Amenity_v2ObjFive.key = "seatRecline";
                    if (SeatDataRSOutbound != null)
                    {
                        Amenity_v2ObjFive.value = SeatDataRSOutbound.ratings.perAttribute.seatRecline.value;
                        Amenity_v2ObjFive.rating = Convert.ToDouble(SeatDataRSOutbound.ratings.perAttribute.seatRecline.rating);
                    }
                    else
                    {
                        Amenity_v2ObjFive.value = RandomValue.Next(1, 5).ToString();
                        Amenity_v2ObjFive.rating = Convert.ToDouble(RandomValue.Next(1, 5));

                        FlightResponse_v2Obj.ErrorList = new List<ErrorRs>();
                        FlightResponse_v2Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound random number generated"));
                    }
                    Amenity_v2List.Add(Amenity_v2ObjFive);

                    Amenity_v2 Amenity_v2ObjSix = new Amenity_v2();
                    Amenity_v2ObjSix.name = "Seat Privacy";
                    Amenity_v2ObjSix.unit = " ";
                    Amenity_v2ObjSix.key = "seatPrivacy";
                    if (SeatDataRSOutbound != null)
                    {
                        Amenity_v2ObjSix.value = SeatDataRSOutbound.ratings.perAttribute.seatPrivacy.value;
                        Amenity_v2ObjSix.rating = Convert.ToDouble(SeatDataRSOutbound.ratings.perAttribute.seatPrivacy.rating);
                    }
                    else
                    {
                        Amenity_v2ObjSix.value = RandomValue.Next(1, 5).ToString();
                        Amenity_v2ObjSix.rating = Convert.ToDouble(RandomValue.Next(1, 5));

                        FlightResponse_v2Obj.ErrorList = new List<ErrorRs>();
                        FlightResponse_v2Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound random number generated"));
                    }
                    Amenity_v2List.Add(Amenity_v2ObjSix);

                    CabinObj.amenities = Amenity_v2List;
                    //CabinObj.rating = await RatingPickup(Convert.ToDouble(Amenity_v2ObjTwo.value) , Convert.ToDouble(Amenity_v2Obj.value));
                    CabinObj.rating = (Amenity_v2ObjTwo.rating + Amenity_v2Obj.rating) / 2;

                    if (SeatDataRSOutbound != null)
                    {
                        CabinObj.pictures = SeatDataRSOutbound.media.pictures.ToList();
                        CabinObj.video = SeatDataRSOutbound.media.video;
                    }

                    Aircraft_v2Obj.cabin = CabinObj;
                    DepartureFlight_v2Obj.aircraft = Aircraft_v2Obj;

                    DepartureFlight_v2Obj.number = Convert.ToInt32(item.outbound.flight);
                    DepartureFlight_v2Obj.numberOfStops = 0;

                    FlightResponse_v2Obj.outbound = DepartureFlight_v2Obj;

                    #endregion

                    /*
                     * Logic for Ratings Arrival Flight
                     */

                    #region Arrival Flight
                    ReturnFlight_v2 ReturnFlight_v2Obj = new ReturnFlight_v2();
                    ReturnFlight_v2Obj.duration = item.inbound.duration;

                    Departure_v2 DepartureReturnFlight_v2Obj = new Departure_v2();
                    DepartureReturnFlight_v2Obj.code = item.inbound.departure.code;
                    DepartureReturnFlight_v2Obj.name = "";
                    DepartureReturnFlight_v2Obj.city = "";
                    DepartureReturnFlight_v2Obj.country = "";
                    DepartureReturnFlight_v2Obj.at = item.inbound.departure.at;
                    ReturnFlight_v2Obj.departure = DepartureReturnFlight_v2Obj;

                    Operating_v2 OperatingReturnFlight_v2Obj = new Operating_v2();
                    OperatingReturnFlight_v2Obj.code = item.inbound.operating.code;
                    OperatingReturnFlight_v2Obj.name = item.inbound.operating.name;
                    ReturnFlight_v2Obj.operating = OperatingReturnFlight_v2Obj;

                    Arrival_v2 ArrivalReturnFlight_v2Obj = new Arrival_v2();
                    ArrivalReturnFlight_v2Obj.code = item.inbound.arrival.code;
                    ArrivalReturnFlight_v2Obj.name = "";
                    ArrivalReturnFlight_v2Obj.city = "";
                    ArrivalReturnFlight_v2Obj.country = "";
                    ArrivalReturnFlight_v2Obj.at = item.inbound.arrival.at;
                    ReturnFlight_v2Obj.arrival = ArrivalReturnFlight_v2Obj;

                    Carrier_v2 CarrierReturnFlight_v2Obj = new Carrier_v2();
                    CarrierReturnFlight_v2Obj.code = item.inbound.carrier.code;
                    CarrierReturnFlight_v2Obj.name = item.inbound.carrier.name;
                    ReturnFlight_v2Obj.carrier = CarrierReturnFlight_v2Obj;

                    Aircraft_v2 AircraftRetunFlight_v2Obj = new Aircraft_v2();
                    AircraftRetunFlight_v2Obj.code = item.inbound.aircraft.code;
                    ReturnFlight_v2Obj.aircraft = AircraftRetunFlight_v2Obj;

                    Cabins_v2 Cabins_v2ObjTwo = new Cabins_v2();
                    Cabins_v2ObjTwo.cabinClass = item.inbound.@class;
                    List<Amenity_v2> Amenity_v2ListTwo = new List<Amenity_v2>();

                    Amenity_v2 Amenity_v2ObjThree = new Amenity_v2();
                    Amenity_v2ObjThree.name = "Seat Width";
                    Amenity_v2ObjThree.unit = "inches";
                    Amenity_v2ObjThree.key = "seatWidth";
                    if (SeatDataRSInbound != null)
                    {
                        Cabins_v2ObjTwo.random = false;
                        Amenity_v2ObjThree.value = SeatDataRSInbound.ratings.perAttribute.seatWidth.value;
                        Amenity_v2ObjThree.rating = Convert.ToDouble(SeatDataRSInbound.ratings.perAttribute.seatWidth.rating);
                    }
                    else
                    {
                        Cabins_v2ObjTwo.random = true;
                        Amenity_v2ObjThree.value = RandomValue.Next(1, 5).ToString() + "," + RandomValue.Next(1, 5).ToString();
                        Amenity_v2ObjThree.rating = Convert.ToDouble(RandomValue.Next(1, 5));

                        FlightResponse_v2Obj.ErrorList = new List<ErrorRs>();
                        FlightResponse_v2Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyInbound.ToString() + "-KeyOutbound random number generated"));
                    }

                    Amenity_v2ListTwo.Add(Amenity_v2ObjThree);

                    Amenity_v2 Amenity_v2ObjFour = new Amenity_v2();
                    Amenity_v2ObjFour.name = "Seat Pitch";
                    Amenity_v2ObjFour.unit = "inches";
                    Amenity_v2ObjFour.key = "seatPitch";
                    if (SeatDataRSInbound != null)
                    {
                        Amenity_v2ObjFour.value = SeatDataRSInbound.ratings.perAttribute.seatPitch.value;
                        Amenity_v2ObjFour.rating = Convert.ToDouble(SeatDataRSInbound.ratings.perAttribute.seatPitch.rating);
                    }
                    else
                    {
                        Amenity_v2ObjFour.value = RandomValue.Next(1, 5).ToString() + "," + RandomValue.Next(1, 5).ToString();
                        Amenity_v2ObjFour.rating = Convert.ToDouble(RandomValue.Next(1, 5));
                        /*Log Outbound.ratings Error */
                        FlightResponse_v2Obj.ErrorList.Add( await _ErrorLog.ErrorLog(TransactionId, KeyInbound.ToString() + "-KeyOutbound random number generated"));
                    }
                    Amenity_v2ListTwo.Add(Amenity_v2ObjFour);

                    Amenity_v2 Amenity_v2ObjSeven = new Amenity_v2();
                    Amenity_v2ObjSeven.name = "Seat Recline";
                    Amenity_v2ObjSeven.unit = " ";
                    Amenity_v2ObjSeven.key = "seatRecline";
                    if (SeatDataRSInbound != null)
                    {
                        Amenity_v2ObjSeven.value = SeatDataRSInbound.ratings.perAttribute.seatRecline.value;
                        Amenity_v2ObjSeven.rating = Convert.ToDouble(SeatDataRSInbound.ratings.perAttribute.seatRecline.rating);
                    }
                    else
                    {
                        Amenity_v2ObjSeven.value = RandomValue.Next(1, 5).ToString();
                        Amenity_v2ObjSeven.rating = Convert.ToDouble(RandomValue.Next(1, 5));

                        FlightResponse_v2Obj.ErrorList = new List<ErrorRs>();
                        FlightResponse_v2Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyInbound.ToString() + "-KeyOutbound random number generated"));
                    }
                    Amenity_v2ListTwo.Add(Amenity_v2ObjSeven);

                    Amenity_v2 Amenity_v2ObjEight = new Amenity_v2();
                    Amenity_v2ObjEight.name = "Seat Privacy";
                    Amenity_v2ObjEight.unit = " ";
                    Amenity_v2ObjEight.key = "seatPrivacy";
                    if (SeatDataRSInbound != null)
                    {
                        Amenity_v2ObjEight.value = SeatDataRSInbound.ratings.perAttribute.seatPrivacy.value;
                        Amenity_v2ObjEight.rating = Convert.ToDouble(SeatDataRSInbound.ratings.perAttribute.seatPrivacy.rating);
                    }
                    else
                    {
                        Amenity_v2ObjEight.value = RandomValue.Next(1, 5).ToString();
                        Amenity_v2ObjEight.rating = Convert.ToDouble(RandomValue.Next(1, 5));

                        FlightResponse_v2Obj.ErrorList = new List<ErrorRs>();
                        FlightResponse_v2Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyInbound.ToString() + "-KeyOutbound random number generated"));
                    }
                    Amenity_v2ListTwo.Add(Amenity_v2ObjEight);


                    Cabins_v2ObjTwo.amenities = Amenity_v2ListTwo;
                    //BusinessObj.rating = await RatingPickup(Convert.ToDouble(Amenity_v2Obj.value), Convert.ToDouble(Amenity_v2ObjTwo.value));
                    Cabins_v2ObjTwo.rating = (Amenity_v2ObjThree.rating + Amenity_v2ObjFour.rating) / 2;

                    if (SeatDataRSInbound != null)
                    {
                        Cabins_v2ObjTwo.pictures = SeatDataRSInbound.media.pictures.ToList();
                        Cabins_v2ObjTwo.video = SeatDataRSInbound.media.video;
                    }

                    AircraftRetunFlight_v2Obj.cabin = Cabins_v2ObjTwo;
                    ReturnFlight_v2Obj.aircraft = AircraftRetunFlight_v2Obj;

                    ReturnFlight_v2Obj.number = Convert.ToInt32(item.inbound.flight);
                    ReturnFlight_v2Obj.numberOfStops = 0;

                    List<Booking> BookingList = new List<Booking>();
                    Booking BookingObj = new Booking();
                    BookingObj.provider = "Expedia";
                    BookingObj.url = "https://www.expedia.com/Flight-Information?journeyContinuationId=AQr1Agq8AnY1LXNvcy03YTc4MzdhNjljOTI0ZjQwODE3NDAzZDY3MzFkZDU4OS0yMC0xOS0yfjIuU35BUW9DQ0FFU0J3alVCQkFCR0FFZ0J5QUJJQXdnRFNBSktBSllBbkFBfkFRcERDaDhJeFpJQkVnTXhNRFFZcVZFZ2d3MG8wLVBjQVRESzV0d0JPRk5BQUZnQkNpQUl4WklCRWdNeE5UUVlndzBnNjRrQktNTG4zQUV3bk9qY0FUaFNRQUJZQVFwRENpQUl4WklCRWdNeE56a1k2NGtCSUlNTktOM3QzQUV3cmU3Y0FUaFVRQUJZQVFvZkNNV1NBUklETVRBMUdJTU5JS2xSS01yMTNBRXdvUG5jQVRoVFFBQllBUklLQ0FJUUFSZ0NLZ0pGU1JnQklnUUlBUkFCS0FJb0F5Z0VNQUkRj8L1KFybj0AaDwgBEgsKBFBUMkgSA0RVQhoTCAISDwoIUFQxNUgyNU0SA0RVQiIBASoCEgASPwoWCgoyMDIwLTExLTE2EgNKRksaA0xPTgoWCgoyMDIwLTExLTE3EgNMT04aA0pGSxIHEgVDT0FDSBoCEAEgAiAB";
                    BookingList.Add(BookingObj);
                    FlightResponse_v2Obj.booking = BookingList;

                    FlightResponse_v2Obj.inbound = ReturnFlight_v2Obj;
                    FlightResponse_v2Obj.cost = item.cost.amount;
                    
                    FlightResponse_v2List.Add(FlightResponse_v2Obj);

                    await _FileSaveCaller.Save_v1(FlightResponse_v2List, "Offer-RS", TransactionId);
                    #endregion

                }

                return await Task.FromResult(FlightResponse_v2List);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation(ErrorMessage.StackTrace);
                _Logger.LogError(ErrorMessage.StackTrace);

            }
            return null;
        }
        #endregion

        #region SeatDataCaller
        public async Task<List<SeatsRs>> SeatDataRequest(List<FlightResponse_v1> FlightResponse_v1List, String TransactionId, string Context)
        {
            #region Fields

            string KeyInbound = "";
            string KeyOutbound = "";
            #endregion

            #region SetSearchKeys
            List<string> SearchKeylist = new List<string>();
            foreach (var item in FlightResponse_v1List)
            {
                KeyInbound = item.inbound.operating.code + "-" + item.inbound.aircraft.code + "-" + "1" + "-" + CabinClassFinder(item.inbound.@class);
                KeyOutbound = item.outbound.operating.code + "-" + item.outbound.aircraft.code + "-" + "1" + "-" + CabinClassFinder(item.outbound.@class);

                if (!SearchKeylist.Contains(KeyInbound))
                {
                    SearchKeylist.Add(KeyInbound);
                }
                if (!SearchKeylist.Contains(KeyOutbound))
                {
                    SearchKeylist.Add(KeyOutbound);
                }

            }
            string[] SearchKey = SearchKeylist.ToArray();
            #endregion

            #region GetSeatData
            List<SeatsRs> SeatDataRSList = new List<SeatsRs>();
            try
            {
                string Key = Context + "@@";
                foreach (var item in SearchKeylist)
                {
                    Key += "@@" + item;
                }

                try
                {
                    SeatDataRSList = await _CacheRepositoryCaller.GetSeats(Key);
                    await _FileSaveCaller.Save_v1(SeatDataRSList, "Cached-Seats-RS", TransactionId);

                }
                catch
                {
                    SeatDataRSList = await _SeatdataCaller.GetSeats(SearchKey, TransactionId, Context);
                    await _FileSaveCaller.Save_v1(SeatDataRSList, "Seats-RS", TransactionId);
                    await _FileSaveCaller.Save_v1(SearchKey, "Seats-RS", TransactionId);

                    await _CacheRepositoryCaller.PostSeats(Key, SeatDataRSList);

                }
                return await Task.FromResult(SeatDataRSList);
            }
            catch
            {

                SeatDataRSList = null;
                return null;
            }
            #endregion
        }
        #endregion

     

        #region CabinClassMapperForSeatData
        /*
       * For replace cabin class which is FCF Format to apply ratings
       * 
       */
        public string CabinClassFinder(String Cabin)
        {
            switch (Cabin.ToUpper())
            {
                case "BUSINESS":
                    Cabin = "J";
                    break;
                case "FIRST":
                    Cabin = "F";
                    break;
                case "PREMIUM_ECONOMY":
                    Cabin = "PE";
                    break;
                case "ECONOMY":
                    Cabin = "Y";
                    break;
                case "ECONOMY+":
                    Cabin = "y+";
                    break;
            }
            return Cabin;
        }
        #endregion

    }
}
