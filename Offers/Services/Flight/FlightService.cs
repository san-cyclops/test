using Microsoft.Extensions.Logging;
using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Flight;
using Offers.Services.Offer;
using Offers.Services.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Flight
{
    public class FlightService : IFlightService<OfferBaseEntity>
    {


        private readonly ILogger<OfferService> _Logger;
        private readonly IOfferService<OfferBaseEntity> _SeatRatingRepositorycaller;
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;
        private readonly ICacheService<CacheBaseEntity> _CacheRepositoryCaller;
        private readonly ISeatService<SeatBaseEntity> _SeatdataCaller;

        public FlightService(IOfferService<OfferBaseEntity> SeatRatingRepositorycaller, ILogger<OfferService> Logger, IFileSaveService<FileSaveBaseEntity> FileSaveCaller, IErrorService<ErrorBaseEntity> ErrorLogCaller, ICacheService<CacheBaseEntity> CacheRepositoryCaller, ISeatService<SeatBaseEntity> SeatdataCaller)
        {
            _Logger = Logger;
            _FileSaveCaller = FileSaveCaller;
            _ErrorLog = ErrorLogCaller;
            _CacheRepositoryCaller = CacheRepositoryCaller;
            _SeatRatingRepositorycaller = SeatRatingRepositorycaller;
            _SeatdataCaller = SeatdataCaller;
        }

        #region FlightBaseCaller
        public async Task<FlightResponse_v4> FlightBaseCaller(string[] uids, string TransactionId, string Context)
        {
            var OfferIdForSave = uids;
            FlightResponse_v3 FlightResponse_v3Obj = new FlightResponse_v3();
            foreach (string item in uids)
            {
                _Logger.LogInformation("Offer-Id >>>>>>>>>> " + item);
                try
                {
                    FlightResponse_v3Obj = await _CacheRepositoryCaller.GetFlight(item);
                    await _FileSaveCaller.Save_v1("uids-" + uids + "@@" + "TransactionId-" + TransactionId , "Cache-Flight-RQ",TransactionId);
                    await _FileSaveCaller.Save_v1(FlightResponse_v3Obj, "Cache-Flight-RS", TransactionId);
                    //FlightResponse_v3List.Add(FlightResponse_v3Obj);
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogWarning("Flight-Id Not in Cache >>>>>>>>>> " + item);
                    _Logger.LogError("Flight-Id Not in cache  >>>>>>>>>> " + ErrorMessage.StackTrace);

                    FlightResponse_v3Obj.ErrorList = new List<ErrorRs>();
                    FlightResponse_v3Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, "Cache Expired - Flight-Id Not in Cache"));
                }

            }
            return await ResponseRatingCalculation(FlightResponse_v3Obj, TransactionId, Context);
        }
        #endregion

        #region LogicForRatingCalculation
        public async Task<FlightResponse_v4> ResponseRatingCalculation(FlightResponse_v3 FlightResponse_v3Obj, String TransactionId, string Context)
        {
            try
            {
                SeatsRs SeatDataRSOutbound = new SeatsRs();
                String KeyOutbound = FlightResponse_v3Obj.operating.code + "-" + FlightResponse_v3Obj.aircraft.code + "-" + "1" + "-" + CabinClassFinder(FlightResponse_v3Obj.cabinClass);

                try
                {
                    List<string> Key = new List<string>();
                    Key.Add(KeyOutbound);
                    SeatDataRSOutbound = await _SeatdataCaller.GetSeat(Key[0], TransactionId, Context);
                }
                catch (Exception ErrorMessage)
                {
                    SeatDataRSOutbound = null;
                    _Logger.LogInformation(ErrorMessage.StackTrace);
                    _Logger.LogError(ErrorMessage.StackTrace);

                }
                Random RandomValue = new Random();


                #region Departure Flight
                FlightResponse_v4 FlightResponse_V4OutboundObj = new FlightResponse_v4();
                FlightResponse_V4OutboundObj.id = FlightResponse_v3Obj.id;
                FlightResponse_V4OutboundObj.duration = FlightResponse_v3Obj.duration;
                Departure_v4 Departure_v4Obj = new Departure_v4();
                Departure_v4Obj.code = FlightResponse_v3Obj.departure.code;
                Departure_v4Obj.name = FlightResponse_v3Obj.departure.name;
                Departure_v4Obj.city = "";
                Departure_v4Obj.country = "";
                Departure_v4Obj.at = FlightResponse_v3Obj.departure.at;
                FlightResponse_V4OutboundObj.departure = Departure_v4Obj;

                Arrival_v4 Arrival_v4Obj = new Arrival_v4();
                Arrival_v4Obj.code = FlightResponse_v3Obj.arrival.code;
                Arrival_v4Obj.name = FlightResponse_v3Obj.arrival.name;
                Arrival_v4Obj.city = "";
                Arrival_v4Obj.country = "";
                Arrival_v4Obj.at = FlightResponse_v3Obj.arrival.at;
                FlightResponse_V4OutboundObj.arrival = Arrival_v4Obj;

                Carrier_v4 Carrier_v4Obj = new Carrier_v4();
                Carrier_v4Obj.code = FlightResponse_v3Obj.carrier.code;
                Carrier_v4Obj.name = FlightResponse_v3Obj.carrier.name;
                FlightResponse_V4OutboundObj.carrier = Carrier_v4Obj;

                Operating_v4 Operating_v4Obj = new Operating_v4();
                Operating_v4Obj.code = FlightResponse_v3Obj.operating.code;
                Operating_v4Obj.name = FlightResponse_v3Obj.operating.name;
                FlightResponse_V4OutboundObj.operating = Operating_v4Obj;

                FlightResponse_V4OutboundObj.numberOfStops = 0;
                FlightResponse_V4OutboundObj.number = FlightResponse_v3Obj.number;
                FlightResponse_V4OutboundObj.cost = FlightResponse_v3Obj.cost;


                Aircraft_v4 Aircraft_v4Obj = new Aircraft_v4();
                Aircraft_v4Obj.code = FlightResponse_v3Obj.aircraft.code;
                Aircraft_v4Obj.name = FlightResponse_v3Obj.aircraft.name;

                Cabin_v4 Cabin_v4Obj = new Cabin_v4();
                Cabin_v4Obj.cabinClass = FlightResponse_v3Obj.cabinClass;
                Cabin_v4Obj.rating = 0;  

                List<Amenity_v4> Amenity_v4List = new List<Amenity_v4>();

                Amenity_v4 Amenity_v4ObjOne = new Amenity_v4();
                Amenity_v4ObjOne.name = "Seat Width";
                Amenity_v4ObjOne.unit = "inches";
                Amenity_v4ObjOne.key = "seatWidth";
                if (SeatDataRSOutbound != null)
                {
                    Amenity_v4ObjOne.value = SeatDataRSOutbound.ratings.perAttribute.seatWidth.value;
                    Amenity_v4ObjOne.rating = Convert.ToDouble(SeatDataRSOutbound.ratings.perAttribute.seatWidth.rating);
                }
                else
                {
                    Amenity_v4ObjOne.value = 0;
                    Amenity_v4ObjOne.rating = 0;
                    FlightResponse_V4OutboundObj.ErrorList = new List<ErrorRs>();
                    FlightResponse_V4OutboundObj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound Seat Width random number generated"));
                }
                Amenity_v4List.Add(Amenity_v4ObjOne);

                Amenity_v4 Amenity_v4ObjTwo = new Amenity_v4();
                Amenity_v4ObjTwo.name = "Seat Legroom";
                Amenity_v4ObjTwo.unit = "inches";
                Amenity_v4ObjTwo.value = "";
                Amenity_v4ObjTwo.key = "Seat Legroom";
                Amenity_v4ObjTwo.rating = 0;
                Amenity_v4List.Add(Amenity_v4ObjTwo);

                FlightResponse_V4OutboundObj.ErrorList = new List<ErrorRs>();
                FlightResponse_V4OutboundObj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound Seat Legroom random number generated"));

                Amenity_v4 Amenity_v4ObjThree = new Amenity_v4();
                Amenity_v4ObjThree.name = "Video Type";
                Amenity_v4ObjThree.key = "videoType";
                Amenity_v4ObjThree.value = "On-Demand TV";
                Amenity_v4ObjThree.rating = 0;
                Amenity_v4List.Add(Amenity_v4ObjThree);

                FlightResponse_V4OutboundObj.ErrorList = new List<ErrorRs>();
                FlightResponse_V4OutboundObj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound Video Type random number generated"));
 
                Amenity_v4 Amenity_v4ObjFour = new Amenity_v4();
                Amenity_v4ObjFour.name = "Laptop Power";
                Amenity_v4ObjFour.unit = " ";
                Amenity_v4ObjFour.key = "laptopPower";
                Amenity_v4ObjFour.value = "All Seats";
                Amenity_v4ObjFour.rating = 0;
                Amenity_v4List.Add(Amenity_v4ObjFour);

                FlightResponse_V4OutboundObj.ErrorList = new List<ErrorRs>();
                FlightResponse_V4OutboundObj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound Laptop Power random number generated"));
 
                Amenity_v4 Amenity_v4ObjFive = new Amenity_v4();
                Amenity_v4ObjFive.name = "Wi-Fi";
                Amenity_v4ObjFive.unit = " ";
                Amenity_v4ObjFive.key = "wiFi";
                Amenity_v4ObjFive.value = "Yes";
                Amenity_v4ObjFive.rating = 0;
                Amenity_v4List.Add(Amenity_v4ObjFive);

                FlightResponse_V4OutboundObj.ErrorList = new List<ErrorRs>();
                FlightResponse_V4OutboundObj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound Wi-Fi random number generated"));


                Amenity_v4 Amenity_v4ObjSix = new Amenity_v4();
                Amenity_v4ObjSix.name = "Seat Recline";
                Amenity_v4ObjSix.unit = " ";
                Amenity_v4ObjSix.key = "seatType";
                if (SeatDataRSOutbound != null)
                {
                    Amenity_v4ObjSix.value = SeatDataRSOutbound.ratings.perAttribute.seatRecline.value;
                    Amenity_v4ObjSix.rating = Convert.ToDouble(SeatDataRSOutbound.ratings.perAttribute.seatRecline.rating);
                }
                else
                {
                    Amenity_v4ObjSix.value = 0;
                    Amenity_v4ObjSix.rating = 0;
                }
                Amenity_v4List.Add(Amenity_v4ObjSix);

                FlightResponse_V4OutboundObj.ErrorList = new List<ErrorRs>();
                FlightResponse_V4OutboundObj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound Seat Recline random number generated"));

                Amenity_v4 Amenity_v4ObjSeven = new Amenity_v4();
                Amenity_v4ObjSeven.name = "Seat Privacy";
                Amenity_v4ObjSeven.unit = " ";
                Amenity_v4ObjSeven.key = "seatPrivacy";
                if (SeatDataRSOutbound != null)
                {
                    Amenity_v4ObjSeven.value = SeatDataRSOutbound.ratings.perAttribute.seatPrivacy.value;
                    Amenity_v4ObjSeven.rating = Convert.ToDouble(SeatDataRSOutbound.ratings.perAttribute.seatPrivacy.rating);
                }
                else
                {
                    Amenity_v4ObjSeven.value = RandomValue.Next(1, 5).ToString();
                    Amenity_v4ObjSeven.rating = Convert.ToDouble(RandomValue.Next(1, 5));

                    FlightResponse_V4OutboundObj.ErrorList = new List<ErrorRs>();
                    FlightResponse_V4OutboundObj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound Seat Privacy random number generated"));
                }
                Amenity_v4List.Add(Amenity_v4ObjSeven);

             
                Cabin_v4Obj.rating = (Amenity_v4ObjOne.rating + Amenity_v4ObjTwo.rating) / 2;

                if (SeatDataRSOutbound != null)
                {
                    Cabin_v4Obj.pictures = SeatDataRSOutbound.media.pictures.ToList();
                }

                Cabin_v4Obj.amenities = Amenity_v4List;
                Aircraft_v4Obj.cabin = Cabin_v4Obj;
                FlightResponse_V4OutboundObj.aircraft = Aircraft_v4Obj;

                #endregion

                return await Task.FromResult(FlightResponse_V4OutboundObj);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation(ErrorMessage.StackTrace);
                _Logger.LogError(ErrorMessage.StackTrace);
            }
            return null;
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
