using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Offers.Models.Amadeus.Search;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Repository.Connecter;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Seat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Services.Offers
{
    /*
     * Methods :SearchBaseCaller/FlightOffersSearch/RequestBuilder - which Create Offers Request which need to send Amadeus Using Api search Query .
     * Methods :ResponseBuilder- Which decode Amadeus response and attached FCF response .
     * Methods :CabinFinder -Method which replace requested cabin for Amadeus format .
     * Methods :MockUI - method which invoke amadeus API and sent response without decoding to the FCF format .
     * Methods :AddRatings - Get and Sets Rating data using FCF formular .
     * Methods :CabinClassMapperForSeatData which used to mapped cabin classes in AddRatings .
     */

    public class OffersService : IOffersService<OffersBaseEntity>
    {
        #region Fields
        private readonly IConfiguration _Configuration;
        private readonly IConnecterRepository<ConnecterBaseEntity> _IAmadeusConnecterCaller;
        private readonly ICacheService<CacheBaseEntity> _CacheCaller;
        private readonly ISeatService<SeatBaseEntity> _SeatdataCaller;
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;

        public ILogger<OffersService> _Logger { get; }
        #endregion

        #region Costructor
        public OffersService(IFileSaveService<FileSaveBaseEntity> FileSaveCaller,IConnecterRepository<ConnecterBaseEntity> IAmadeusConnecterCaller, IConfiguration Configuration, ILogger<OffersService> Logger, ICacheService<CacheBaseEntity> CacheCaller, ISeatService<SeatBaseEntity> SeatdataCaller, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _Configuration = Configuration;
            _IAmadeusConnecterCaller = IAmadeusConnecterCaller;
            _Logger = Logger;
            _CacheCaller = CacheCaller;
            _SeatdataCaller = SeatdataCaller;
            _FileSaveCaller = FileSaveCaller;
            _ErrorLog = ErrorLogCaller;

        }
        #endregion

        #region SearchBaseCaller
        /*
         * Take FlightSearchQuerystring and build Amadeus Format and Send to the SupplierConnecter ,then got results and return Resopnse acording to the FC F format v1
         * 
         */
        public async Task<List<FlightResponse_v1>> FlightOffersSearch([FromQuery] FlightSearchQuery_v1 RequestQuery)
       {
            /*
            * Format Of data : MIA-LHR, Apr 1/Apr 8, Bus/PE ,Note should agree how front end send these cabin data EX: BUS/PEC/ECO/FIR
            * Tracking should be implement once replace raw data 
           */
            try
            {
                /* The amount of cabin classes (RequestQuery.Cabins) mapped with queris which need to send amadeus for results .
                 */
                #region Calling to Supplier
                List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();
                await _FileSaveCaller.Save_v1(RequestQuery, "Offfers-RQ", RequestQuery.TransactionId);

               
                foreach (var item in RequestQuery.Cabins.Split(','))
                {
                    List<FlightResponse_v1> FlightResponse_v1Obj = new List<FlightResponse_v1>();
                    RequestQuery.Cabins = item;

                    Random RandomNumber = new Random();
                    
                    string TransactionId = RequestQuery.TransactionId + "_" + item+"_"+ RequestQuery.DepatureLocation+"_"+ RequestQuery.ArrivalLocation;

                    String BaseKey = RequestQuery.DepartureDate + "@@" + RequestQuery.ArrivalDate + "@@" + RequestQuery.DepatureLocation + "@@" + RequestQuery.ArrivalLocation + "@@" + item + "@@" + RequestQuery.PaxAdult + "@@" + RequestQuery.PaxChild + "@@" + RequestQuery.DateFlexibility;

                    try
                    {
                        FlightResponse_v1Obj = await _CacheCaller.GetOffers(BaseKey);
                        await _FileSaveCaller.Save_v1(BaseKey, "Cache-RQ", TransactionId);
                        await _FileSaveCaller.Save_v1(FlightResponse_v1Obj, "Cache-RS",TransactionId);
                        FlightResponse_v1List.AddRange(FlightResponse_v1Obj);
                    }
                    catch 
                    {
                        FlightOffersSearchRq FlightSearchRQObj = await FlightSearchRQBuilder(RequestQuery);
                        FlightResponse_v1Obj = await FlightResponseBuilderV1(await _IAmadeusConnecterCaller.GetOffers(FlightSearchRQObj, TransactionId));

                        if (RequestQuery.DateFlexibility!=0)
                        {
                            FlightResponse_v1Obj = await DateRangeChecker(FlightResponse_v1Obj, RequestQuery.DateFlexibility);
                        }
                        

                        if (FlightResponse_v1Obj!=null)
                        {
                            FlightResponse_v1Obj = await RatingsBuilderV1(FlightResponse_v1Obj,TransactionId, RequestQuery.Context);
                            await _FileSaveCaller.Save_v1(FlightResponse_v1Obj, "Offfers-RS", TransactionId);
                            if (FlightResponse_v1Obj !=null)
                            {
                                FlightResponse_v1List.AddRange(FlightResponse_v1Obj);
                            }
                        }
                        try
                        {
                                await _CacheCaller.PostOffers(FlightResponse_v1Obj, BaseKey);
                                await _FileSaveCaller.Save_v1(BaseKey, "Cache-RQ", TransactionId);
                                await _FileSaveCaller.Save_v1(FlightResponse_v1Obj, "Cache-RS", TransactionId);
                        }
                        catch (Exception Error)
                        {
                             await _FileSaveCaller.Save_v1(Error, "Error-Offers", "" + "_" + TransactionId);
                            _Logger.LogInformation("FlightOffersSearch Error because of exception at caching OffersService");
                            _Logger.LogError("FlightOffersSearch Error because of exception at caching", Error.StackTrace);
                        }
                    }
                }
                return FlightResponse_v1List;
                #endregion
            }
            catch (Exception Error)
            {
                 await _FileSaveCaller.Save_v1(Error, "Error-Offers", "" + "_" + RequestQuery.TransactionId);
                _Logger.LogInformation("FlightOffersSearch Error because of exception at OffersService");
                _Logger.LogError("FlightOffersSearch Error", Error.StackTrace);
                return null;
            }

        }
        #endregion

        #region MockUI
        /*
       * Provide Amadeus original response without decoding 
       * 
       */
        public async Task<List<FlightOffersSearchRs>> FlightOffersSearchock([FromQuery] FlightSearchQuery_v1 RequestQuery)
        {
            /*
            * Format Of data : MIA-LHR, Apr 1/Apr 8, Bus/PE ,Note should agree how front end send these cabin data EX: BUS/PEC/ECO/FIR
            * Tracking should be implement once replace raw data 
           */
            try
            {
                #region Calling to Supplier
                //FlightSearchRQ FlightSearchRQObj = await FlightSearchRQBuilder(RequestQuery);
                //FlightOffersSearchRS FlightOffersSearchRSObj = await _IAmadeusConnecterCaller.PostSearch(FlightSearchRQObj);
                //return FlightOffersSearchRSObj;

                List<FlightOffersSearchRs> FlightResponse_v1List = new List<FlightOffersSearchRs>();
                foreach (var item in RequestQuery.Cabins.Split(','))
                {
                    RequestQuery.Cabins = item;
                    List<FlightOffersSearchRs> FlightResponse_v1ListTwo = new List<FlightOffersSearchRs>();
                    FlightOffersSearchRq FlightSearchRQObj = await FlightSearchRQBuilder(RequestQuery);
                    FlightOffersSearchRs FlightResponse_v1Obj = await _IAmadeusConnecterCaller.GetOffers(FlightSearchRQObj, RequestQuery.TransactionId);
                    FlightResponse_v1ListTwo.Add(FlightResponse_v1Obj);
                    FlightResponse_v1List.AddRange(FlightResponse_v1ListTwo);
                }
                return FlightResponse_v1List;


                #endregion
            }
            catch (Exception Error)
            {
                _Logger.LogError("FlightOffersSearch Error", Error);
                return null;
            }

        }
        #endregion

        #region RequestBuilder
        /*
       * Create Request according to Amadeus Format after we got FlightSearchQuery From Front-end
       * 
       */
        public Task<FlightOffersSearchRq> FlightSearchRQBuilder([FromQuery] FlightSearchQuery_v1 RequestQuery)
        {

            try
            {
                FlightOffersSearchRq FlightSearchRQ = new FlightOffersSearchRq();
                FlightSearchRQ.currencyCode = "USD";

                List<OriginDestination> OriginDestinationList = new List<OriginDestination>();
                OriginDestination OriginDestinationObj = new OriginDestination();
                OriginDestinationObj.id = "1";
                OriginDestinationObj.originLocationCode = RequestQuery.DepatureLocation.ToUpper();
                OriginDestinationObj.destinationLocationCode = RequestQuery.ArrivalLocation.ToUpper();

                DepartureDateTimeRange DepartureDateTimeRangeObj = new DepartureDateTimeRange();
                DepartureDateTimeRangeObj.date = RequestQuery.DepartureDate;
                DepartureDateTimeRangeObj.time = "00:00:00";
                if (RequestQuery.DateFlexibility > 0)
                {
                    DepartureDateTimeRangeObj.dateWindow = "I" + RequestQuery.DateFlexibility + "D";
                }
                OriginDestinationObj.departureDateTimeRange = DepartureDateTimeRangeObj;
                OriginDestinationList.Add(OriginDestinationObj);

                OriginDestination OriginDestinationObjTwo = new OriginDestination();
                OriginDestinationObjTwo.id = "2";
                OriginDestinationObjTwo.originLocationCode = RequestQuery.ArrivalLocation.ToUpper();
                OriginDestinationObjTwo.destinationLocationCode = RequestQuery.DepatureLocation.ToUpper();

                DepartureDateTimeRange DepartureDateTimeRangeObjTwo = new DepartureDateTimeRange();
                DepartureDateTimeRangeObjTwo.date = RequestQuery.ArrivalDate;
                DepartureDateTimeRangeObjTwo.time = "00:00:00";
                if (RequestQuery.DateFlexibility > 0)
                {
                    DepartureDateTimeRangeObjTwo.dateWindow = "I" + RequestQuery.DateFlexibility + "D";
                }
                OriginDestinationObjTwo.departureDateTimeRange = DepartureDateTimeRangeObjTwo;
                OriginDestinationList.Add(OriginDestinationObjTwo);

                FlightSearchRQ.originDestinations = OriginDestinationList;

                List<Traveler> TravelerList = new List<Traveler>();
                int TravelerCount = 1;
                if (RequestQuery.PaxAdult > 0)
                {
                    for (int i = 0; i < RequestQuery.PaxAdult; i++)
                    {
                        Traveler TravelerObj = new Traveler();
                        TravelerObj.id = TravelerCount.ToString();
                        TravelerObj.travelerType = "ADULT";
                        TravelerList.Add(TravelerObj);
                        TravelerCount++;
                    }

                }

                if (RequestQuery.PaxChild > 0)
                {
                    for (int i = 0; i < RequestQuery.PaxAdult; i++)
                    {
                        Traveler TravelerObj = new Traveler();
                        TravelerObj.id = TravelerCount.ToString();
                        TravelerObj.travelerType = "CHILD";
                        TravelerList.Add(TravelerObj);
                        TravelerCount++;
                    }
                }

                FlightSearchRQ.travelers = TravelerList;

                List<string> Sources = new List<string>();
                Sources.Add("GDS");
                FlightSearchRQ.sources = Sources;

                SearchCriteria SearchCriteriaObj = new SearchCriteria();
                SearchCriteriaObj.maxFlightOffers = 200;
                FlightFilters FlightFiltersObj = new FlightFilters();

                List<CabinRestriction> CabinRestrictionList = new List<CabinRestriction>();
                CabinRestriction CabinRestrictionObj = new CabinRestriction();
                CabinRestrictionObj.cabin = CabinFinder(RequestQuery.Cabins.Substring(0, 1));
                CabinRestrictionObj.coverage = "ALL_SEGMENTS";
                List<string> OriginDestinationIdsObj = new List<string>();
                OriginDestinationIdsObj.Add("1");
                CabinRestrictionObj.originDestinationIds = OriginDestinationIdsObj;
                CabinRestrictionList.Add(CabinRestrictionObj);

                CabinRestriction CabinRestrictionObjTwo = new CabinRestriction();

                CabinRestrictionObjTwo.cabin = CabinFinder(RequestQuery.Cabins.Substring(1, 1));
                CabinRestrictionObjTwo.coverage = "ALL_SEGMENTS";
                List<string> OriginDestinationIdsTwo = new List<string>();
                OriginDestinationIdsTwo.Add("2");
                CabinRestrictionObjTwo.originDestinationIds = OriginDestinationIdsTwo;
                CabinRestrictionList.Add(CabinRestrictionObjTwo);

                connectionRestriction connectionRestrictionObj = new connectionRestriction();
                connectionRestrictionObj.maxNumberOfConnections = 0;

                FlightFiltersObj.connectionRestriction = connectionRestrictionObj;
                FlightFiltersObj.cabinRestrictions = CabinRestrictionList;
                SearchCriteriaObj.flightFilters = FlightFiltersObj;

                FlightSearchRQ.searchCriteria = SearchCriteriaObj;
                return Task.FromResult(FlightSearchRQ);
            }
            catch (Exception Error)
            {
                _Logger.LogError("FlightSearchRQBuilder Error", Error);
                return null;
            }

        }
        #endregion

        #region CabinFinder
        /*
       * For replace cabin class which is we got from search query This method will invoke in FlightSearchRQBuilder
       * 
       */
        public string CabinFinder(String Cabin)
        {
            switch (Cabin.ToUpper())
            {
                case "B":
                    Cabin = "BUSINESS";
                    break;
                case "F":
                    Cabin = "FIRST";
                    break;
                case "P":
                    Cabin = "PREMIUM_ECONOMY";
                    break;
                case "E":
                    Cabin = "ECONOMY";
                    break;
            }
            return Cabin;
        }
        #endregion

        #region ResponseBuilder
        /*
         * Response Decode once we got response from Amadeus .This method will Invoke FlightOffersSearch Method
         * 
         */

        public Task<List<FlightResponse_v1>> FlightResponseBuilderV1(FlightOffersSearchRs FlightOffersSearchRSObj)
        {
            List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();
            try
            {
              if (FlightOffersSearchRSObj.data != null)
              {
                foreach (var Data in FlightOffersSearchRSObj.data)
                {
                    FlightResponse_v1 FlightResponseObj = new FlightResponse_v1();
                    FlightResponseObj.id = Data.id + DateTimeOffset.Now.ToUnixTimeMilliseconds() + new Random().Next(int.MinValue, int.MaxValue);
                    List<Itinerary> Itineraries = Data.itineraries;

                    // Set out bound departure object
                    Outbound Departurev1Obj = new Outbound();
                    var DepatureSegmentId = Itineraries[0].segments[0].id;
                    Departurev1Obj.flight = Itineraries[0].segments[0].number;

                    Departure_v1 DepartureOut = new Departure_v1();
                    DepartureOut.code = Itineraries[0].segments[0].departure.iataCode;
                    DepartureOut.name = "";
                    DepartureOut.at = Itineraries[0].segments[0].departure.at.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                    Departurev1Obj.departure = DepartureOut;

                    Arrival_v1 ArrivalOut = new Arrival_v1();
                    ArrivalOut.code = Itineraries[0].segments[0].arrival.iataCode;
                    ArrivalOut.name = "";
                    ArrivalOut.at = Itineraries[0].segments[0].arrival.at.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                    Departurev1Obj.arrival = ArrivalOut;

                    Carrier_v1 CarrierOut = new Carrier_v1();
                    CarrierOut.code = Itineraries[0].segments[0].carrierCode;
                    CarrierOut.name = FlightOffersSearchRSObj.dictionaries.carriers[Itineraries[0].segments[0].carrierCode.ToString()];
                    Departurev1Obj.carrier = CarrierOut;

                    Operating_v1 OperatingOut = new Operating_v1();
                    OperatingOut.code = Itineraries[0].segments[0].operating.carrierCode;
                    OperatingOut.name = FlightOffersSearchRSObj.dictionaries.carriers[Itineraries[0].segments[0].operating.carrierCode.ToString()];
                    Departurev1Obj.operating = OperatingOut;

                    Aircraft_v1 AircraftOut = new Aircraft_v1();
                    AircraftOut.code = Itineraries[0].segments[0].aircraft.code;
                    AircraftOut.name = FlightOffersSearchRSObj.dictionaries.aircraft[Itineraries[0].segments[0].aircraft.code.ToString()];
                    Departurev1Obj.aircraft = AircraftOut;

                    Departurev1Obj.duration = Itineraries[0].duration;


                    Ratings RatingsObjDepature = new Ratings();
                    RatingsObjDepature.seatPitch = 0.0;
                    RatingsObjDepature.seatWidth = 0.0;
                    RatingsObjDepature.laptopPower = 0.0;
                    RatingsObjDepature.seatType = 0.0;
                    RatingsObjDepature.seatPrivacy = 0.0;
                    RatingsObjDepature.videoType = 0.0;
                    RatingsObjDepature.wiFi = 0.0;

                    Departurev1Obj.ratings = RatingsObjDepature;

                    // Set In bound departure object 
                    Inbound ReturnObj = new Inbound();
                    var ReturnSegmentId = Itineraries[1].segments[0].id;
                    ReturnObj.flight = Itineraries[1].segments[0].number;

                    Departure_v1 DepartureIn = new Departure_v1();
                    DepartureIn.code = Itineraries[1].segments[0].departure.iataCode;
                    DepartureIn.name = "";
                    DepartureIn.at = Itineraries[1].segments[0].departure.at.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                    ReturnObj.departure = DepartureIn;

                    Arrival_v1 ArrivalIn = new Arrival_v1();
                    ArrivalIn.code = Itineraries[1].segments[0].arrival.iataCode;
                    ArrivalIn.name = "";
                    ArrivalIn.at = Itineraries[1].segments[0].arrival.at.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
                    ReturnObj.arrival = ArrivalIn;

                    Carrier_v1 CarrierIn = new Carrier_v1();
                    CarrierIn.code = Itineraries[1].segments[0].carrierCode;
                    CarrierIn.name = FlightOffersSearchRSObj.dictionaries.carriers[Itineraries[1].segments[0].carrierCode.ToString()];
                    ReturnObj.carrier = CarrierIn;

                    Operating_v1 OperatingIn = new Operating_v1();
                    OperatingIn.code = Itineraries[1].segments[0].operating.carrierCode;
                    OperatingIn.name = FlightOffersSearchRSObj.dictionaries.carriers[Itineraries[1].segments[0].operating.carrierCode.ToString()];
                    ReturnObj.operating = OperatingIn;

                    Aircraft_v1 AircraftIn = new Aircraft_v1();
                    AircraftIn.code = Itineraries[1].segments[0].aircraft.code;
                    AircraftIn.name = FlightOffersSearchRSObj.dictionaries.aircraft[Itineraries[1].segments[0].aircraft.code.ToString()];
                    ReturnObj.aircraft = AircraftIn;

                    ReturnObj.duration = Itineraries[1].duration;

                    Ratings RatingsObjReturn = new Ratings();
                    RatingsObjReturn.seatPitch = 0.0;
                    RatingsObjReturn.seatWidth = 0.0;
                    RatingsObjReturn.laptopPower = 0.0;
                    RatingsObjReturn.seatType = 0.0;
                    RatingsObjReturn.seatPrivacy = 0.0;
                    RatingsObjReturn.videoType = 0.0;
                    RatingsObjReturn.wiFi = 0.0;

                    ReturnObj.ratings = RatingsObjReturn;

                    List<TravelerPricing> TravelPricing = Data.travelerPricings;
                    List<FareDetailsBySegment> Pricing = TravelPricing[0].fareDetailsBySegment;

                    Cost CosObj = new Cost();
                    CosObj.unit = Data.price.currency;
                    CosObj.amount = Convert.ToDouble(Data.price.total);

                    FlightResponseObj.cost = CosObj;
                    if (Pricing[0].segmentId == DepatureSegmentId)
                    {
                        Departurev1Obj.@class = Pricing[0].cabin;
                    };

                    if (Pricing[1].segmentId == ReturnSegmentId)
                    {
                        ReturnObj.@class = Pricing[1].cabin;
                    };
                    FlightResponseObj.inbound = ReturnObj;
                    FlightResponseObj.outbound = Departurev1Obj;
                    if (FlightResponseObj!=null)
                    {
                       FlightResponse_v1List.Add(FlightResponseObj);
                    }
                }

              }
                return Task.FromResult(FlightResponse_v1List);
            }
            catch (Exception Error)
            {
                _Logger.LogError("FlightResponseBuilder Error", Error);
                return null;
            }
        }
        #endregion

        #region DateRangeChecker
        public async Task<List<FlightResponse_v1>> DateRangeChecker(List<FlightResponse_v1> FlightOffersSearchRSObj,int DateFlex)
        {
            List<FlightResponse_v1> ReturnList = new List<FlightResponse_v1>();
            int DateCount = DateRange(DateFlex);
            foreach (FlightResponse_v1 item in FlightOffersSearchRSObj)
            {
                if(((int)DateTime.Parse(item.inbound.departure.at.Substring(0, 10)).Subtract(DateTime.Parse(item.outbound.departure.at.Substring(0, 10))).TotalDays)== DateCount)
                {
                    ReturnList.Add(item);
                }
            }
            return await Task.FromResult(ReturnList);
        }
        #endregion

        #region DateRange
        public int DateRange(int DateRange)
        {
            switch (DateRange)
            {
                case 3:
                    DateRange = 7;
                    break;
                case 2:
                    DateRange = 5;
                    break;
                case 1:
                    DateRange = 3;
                    break;
            }
            return DateRange;
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
                    SeatDataRSList = await _CacheCaller.GetSeats(Key);
                    await _FileSaveCaller.Save_v1(SeatDataRSList, "Cached-Seats-RS", TransactionId);

                }
                catch
                {
                    SeatDataRSList = await _SeatdataCaller.GetSeats(SearchKey, TransactionId, Context);
                    await _FileSaveCaller.Save_v1(SeatDataRSList, "Seats-RS", TransactionId);
                    await _FileSaveCaller.Save_v1(SearchKey, "Seats-RS", TransactionId);

                    await _CacheCaller.PostSeats(Key, SeatDataRSList);

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

        #region AddRatings
        public async Task<List<FlightResponse_v1>> RatingsBuilderV1(List<FlightResponse_v1> FlightResponse_v1List,String TransactionId,string Context)
        {
            List<FlightResponse_v1> FlightResponse_v1SeatdataList = new List<FlightResponse_v1>();
            List<SeatsRs> SeatDataRSList = new List<SeatsRs>();

            #region NullCatcher
            if (FlightResponse_v1List.Count== 0)
            {
                ErrorRs ErrorRs = new ErrorRs();
                //ErrorRs.code = 100;
                ErrorRs.detail = "Flight data list contaion null FlightResponse TransactionId= " + TransactionId;
                ErrorRs.title = "Offer Service Error -" + TransactionId;

                return null;
            }
            #endregion

            SeatDataRSList = await SeatDataRequest(FlightResponse_v1List, TransactionId, Context);

            #region ApplySeatData
            try
            {
                foreach (FlightResponse_v1 FlightResponse_v1Obj in FlightResponse_v1List)
                {
                    if (FlightResponse_v1Obj!=null)
                    {
                       string KeyInbound = FlightResponse_v1Obj.inbound.operating.code + "-" + FlightResponse_v1Obj.inbound.aircraft.code + "-" + "1" + "-" + CabinClassFinder(FlightResponse_v1Obj.inbound.@class);
                       string KeyOutbound = FlightResponse_v1Obj.outbound.operating.code + "-" + FlightResponse_v1Obj.outbound.aircraft.code + "-" + "1" + "-" + CabinClassFinder(FlightResponse_v1Obj.outbound.@class);

                        SeatsRs SeatDataRSInboundObj = new SeatsRs();
                        try
                        {
                            SeatDataRSInboundObj = SeatDataRSList.Find(RatingInbound => RatingInbound.id == KeyInbound);
                        }
                        catch (Exception ErrorMessage)
                        {
                            _Logger.LogInformation("SeatDataRSInboundObj Null for key = " + KeyInbound);
                            _Logger.LogError("FlightOffersSearch Error", ErrorMessage.StackTrace);
                            SeatDataRSInboundObj = null;
                        }

                        Random Randomvalue = new Random();
                        try
                        {
                            if (SeatDataRSInboundObj != null)
                            {
                                if (SeatDataRSInboundObj.ratings != null)
                                {
                                    FlightResponse_v1Obj.inbound.ratings.random = false;
                                    FlightResponse_v1Obj.inbound.ratings.seatPitch = Convert.ToDouble(SeatDataRSInboundObj.ratings.perAttribute.seatPitch.rating);
                                    FlightResponse_v1Obj.inbound.ratings.seatWidth = Convert.ToDouble(SeatDataRSInboundObj.ratings.perAttribute.seatWidth.rating);
                                    FlightResponse_v1Obj.inbound.ratings.seatRecline = Convert.ToDouble(SeatDataRSInboundObj.ratings.perAttribute.seatRecline.rating);
                                    FlightResponse_v1Obj.inbound.ratings.seatPrivacy = Convert.ToDouble(SeatDataRSInboundObj.ratings.perAttribute.seatPrivacy.rating);
                                }
                                else
                                {
                                    FlightResponse_v1Obj.inbound.ratings.random = true;
                                    FlightResponse_v1Obj.inbound.ratings.seatPitch = Convert.ToDouble(Randomvalue.Next(1, 5));
                                    FlightResponse_v1Obj.inbound.ratings.seatWidth = Convert.ToDouble(Randomvalue.Next(1, 5));
                                    FlightResponse_v1Obj.inbound.ratings.seatRecline = Convert.ToDouble(Randomvalue.Next(1, 5));
                                    FlightResponse_v1Obj.inbound.ratings.seatPrivacy = Convert.ToDouble(Randomvalue.Next(1, 5));
                                    _Logger.LogWarning("KeyInbound Seats = " + KeyInbound);

                                    FlightResponse_v1Obj.ErrorList = new List<ErrorRs>();
                                    FlightResponse_v1Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyInbound.ToString() + "-KeyOutbound random number generated"));
                                }
                            }
                            else
                            {
                                FlightResponse_v1Obj.inbound.ratings.random = true;
                                FlightResponse_v1Obj.inbound.ratings.seatPitch = Convert.ToDouble(Randomvalue.Next(1, 5));
                                FlightResponse_v1Obj.inbound.ratings.seatWidth = Convert.ToDouble(Randomvalue.Next(1, 5));
                                FlightResponse_v1Obj.inbound.ratings.seatRecline = Convert.ToDouble(Randomvalue.Next(1, 5));
                                FlightResponse_v1Obj.inbound.ratings.seatPrivacy = Convert.ToDouble(Randomvalue.Next(1, 5));
                                _Logger.LogWarning("KeyInbound Seats = " + KeyInbound);

                                FlightResponse_v1Obj.ErrorList = new List<ErrorRs>();
                                FlightResponse_v1Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyInbound.ToString() + "-KeyOutbound random number generated"));
                            }
                        }
                        catch (Exception ErrorMessage)
                        {
                            _Logger.LogWarning("KeyInbound = " + KeyInbound);
                            _Logger.LogError(ErrorMessage.StackTrace);
                        }
                            
                        SeatsRs SeatDataRSOutboundObj = new SeatsRs();
                        try
                        {
                                SeatDataRSOutboundObj = SeatDataRSList.Find(RatingOutbound => RatingOutbound.id == KeyOutbound);
                        }
                        catch (Exception ErrorMessage)
                        {
                            _Logger.LogInformation("SeatDataRSInboundObj Null for key = "+KeyOutbound);
                            _Logger.LogError("FlightOffersSearch Error", ErrorMessage.StackTrace);
                            SeatDataRSOutboundObj = null;
                        }
                        try
                        {
                            if (SeatDataRSOutboundObj != null)
                            {
                                if (SeatDataRSOutboundObj.ratings != null)
                                {
                                    FlightResponse_v1Obj.outbound.ratings.random = false;
                                    FlightResponse_v1Obj.outbound.ratings.seatPitch = Convert.ToDouble(SeatDataRSOutboundObj.ratings.perAttribute.seatPitch.rating);
                                    FlightResponse_v1Obj.outbound.ratings.seatWidth = Convert.ToDouble(SeatDataRSOutboundObj.ratings.perAttribute.seatWidth.rating);
                                    FlightResponse_v1Obj.outbound.ratings.seatRecline = Convert.ToDouble(SeatDataRSOutboundObj.ratings.perAttribute.seatRecline.rating);
                                    FlightResponse_v1Obj.outbound.ratings.seatPrivacy = Convert.ToDouble(SeatDataRSOutboundObj.ratings.perAttribute.seatPrivacy.rating);
                                }
                                else
                                {
                                    FlightResponse_v1Obj.outbound.ratings.random = true;
                                    FlightResponse_v1Obj.outbound.ratings.seatPitch = Convert.ToDouble(Randomvalue.Next(1, 5));
                                    FlightResponse_v1Obj.outbound.ratings.seatWidth = Convert.ToDouble(Randomvalue.Next(1, 5));
                                    FlightResponse_v1Obj.outbound.ratings.seatRecline = Convert.ToDouble(Randomvalue.Next(1, 5));
                                    FlightResponse_v1Obj.outbound.ratings.seatPrivacy = Convert.ToDouble(Randomvalue.Next(1, 5));
                                    _Logger.LogWarning("KeyOutbound = " + KeyOutbound);

                                    FlightResponse_v1Obj.ErrorList = new List<ErrorRs>();
                                    FlightResponse_v1Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound random number generated"));
                                }
                            }
                            else
                            {
                                FlightResponse_v1Obj.outbound.ratings.random = true;
                                FlightResponse_v1Obj.outbound.ratings.seatPitch = Convert.ToDouble(Randomvalue.Next(1, 5));
                                FlightResponse_v1Obj.outbound.ratings.seatWidth = Convert.ToDouble(Randomvalue.Next(1, 5));
                                FlightResponse_v1Obj.outbound.ratings.seatRecline = Convert.ToDouble(Randomvalue.Next(1, 5));
                                FlightResponse_v1Obj.outbound.ratings.seatPrivacy = Convert.ToDouble(Randomvalue.Next(1, 5));
                                _Logger.LogWarning("KeyOutbound = " + KeyOutbound);

                                FlightResponse_v1Obj.ErrorList = new List<ErrorRs>();
                                FlightResponse_v1Obj.ErrorList.Add(await _ErrorLog.ErrorLog(TransactionId, KeyOutbound.ToString() + "-KeyOutbound random number generated"));
                            }
                        }
                        catch (Exception e)
                        {
                            _Logger.LogWarning("KeyOutbound = " + KeyOutbound + " - " + e.Message);
                        }
                       
                        FlightResponse_v1SeatdataList.Add(FlightResponse_v1Obj);
                    }
                   
                }
                return await Task.FromResult(FlightResponse_v1SeatdataList);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation("Exception in Ofes rating builder");
                _Logger.LogError(ErrorMessage.StackTrace);
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
